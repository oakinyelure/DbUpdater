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
            MigrationContext = new TypeLookup().GetInstanceByFullName<DbContext>(_serviceScope, _options.Context);
            PersistMigration();
            PersistScript();
            PersistSeed();            
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
            // Find where the data context assembly is. We only want to execute the seeder in that assembly
            Assembly assembly = new TypeLookup().TryGetByFullName(_options.Context).GetType().Assembly;
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
                    Console.WriteLine($"Executing seed for {seeder.GetType().FullName}");
                    seeder.Seed(_serviceScope);
                });
            }
            else
            {
                Console.WriteLine($"No seeder found. Skipping.");
            }
        }
    }
}
