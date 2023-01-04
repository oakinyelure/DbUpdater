using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace DbUpdater.EFCore.CLI
{
    /// <summary>
    /// Implementation of the DbContext migration class for persisting code-first entities into the
    /// database. This includes actual migration and logical seeding
    /// </summary>
    internal sealed class DbContextMigration : IContextDeployable, IContextSeedable
    {
        private readonly MigrationOption _options;
        private readonly IServiceScope _serviceScope;

        internal DbContext MigrationContext;

        /// <summary>
        /// Fetches an instance of a class by name from the service provider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private T TryGetDBContextFromServiceProvider<T>()
        {
            // Crawl through the entire assembly to get the type that matches the
            // context argument
            var typeOfContext = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(e => e.GetTypes())
                .FirstOrDefault(e => e.FullName.Equals(_options.Context,StringComparison.OrdinalIgnoreCase) && e.IsAssignableFrom(typeof(T)));
            if (typeOfContext == null) throw new Exception("No matching context exist in the assembly");
            
            // Get the registered type from the service provider
            var contextType = _serviceScope.ServiceProvider.GetService(typeOfContext);
            if (contextType == null) throw new Exception("No matching DbContext type found in the service collection");
            return (T)contextType;
        }

        /// <summary>
        /// Instance of the context migration
        /// </summary>
        /// <param name="host"></param>
        /// <param name="opt"></param>
        public DbContextMigration(IHost host, MigrationOption opt)
        {
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();
            using IServiceScope serviceScope = scopeFactory.CreateScope();
            _serviceScope = serviceScope;
            _options = opt;
            MigrationContext = TryGetDBContextFromServiceProvider<DbContext>();            
            PersistMigration();
        }

        /// inheritDoc
        public void PersistMigration()
        {
            if (!_options.Migrate)
            {
                Console.WriteLine("Caller chose to skip migration because it's not included in the command call.");
                return;
            }
            var pendingMigrations = MigrationContext.Database.GetPendingMigrations().Count();
            if (pendingMigrations <= 0)
            {
                Console.WriteLine("Caller choose to skip migration because there is no pending migration");
                return;
            }
            var existingMigrations = MigrationContext.Database.GetAppliedMigrations().Count();
            Console.WriteLine($"Applied {existingMigrations} till date. Applying {pendingMigrations} now.....");
            MigrationContext.Database.Migrate();
            PersistScript();
        }

        /// <summary>
        /// Persists SQL script into the database
        /// </summary>
        public void PersistScript()
        {
            if (!_options.Scripts.Any())
            {
                Console.WriteLine("No attached script to be propalgated. Moving on");
                return;
            }
            foreach (var item in _options.Scripts)
            {
                var file = new FileInfo(item);
                if (file.Exists && file.FullName.EndsWith(".sql"))
                {
                    string scriptContext = File.ReadAllText(item);
                    int recordAffected = MigrationContext.Database.ExecuteSqlRaw(scriptContext);
                    Console.WriteLine($"{file.FullName} executed. Affects {recordAffected} records");
                }
            }
        }

        /// inheritDoc
        public void PersistSeed()
        {
            if (!_options.Seed)
            {
                Console.WriteLine("Seeding is not elected. Skipping");
                return;
            }
            // Find where the dbContext assembly is. We only want to execute the seeder in that assembly
            Assembly assembly = MigrationContext.GetType().Assembly;
            var seeders = assembly.GetTypes()
                .Where(type => !type.IsAbstract && typeof(AbstractContextSeeder).IsAssignableFrom(type))
                .Select(type => Activator.CreateInstance(type) as AbstractContextSeeder)
                .OrderBy(type => type.Order)
                .ToList();
            if (seeders.Any())
            {
                Console.WriteLine($"Found {seeders.Count} seeders....");
                seeders.ForEach(seeder =>
                {
                    Console.WriteLine($"Executing {seeder.GetType().FullName}");
                    seeder.Seed(_serviceScope);
                });
            }
        }
    }
}
