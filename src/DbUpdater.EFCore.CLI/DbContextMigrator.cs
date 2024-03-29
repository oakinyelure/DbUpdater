﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            MigrationContext = new TypeLookup().GetServiceProviderInstanceByFullName<DbContext>(_serviceScope, _options.Context);
            PersistMigration();
            PersistScript();
            PersistSeed();            
        }

        /// <inheritdoc/>
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
                    try
                    {
                        Console.WriteLine("Executing {0}. Please wait...",file.FullName);
                        string scriptContext = File.ReadAllText(item);
                        MigrationContext.Database.ExecuteSqlRaw(scriptContext); 
                        
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("An error have occurred. Moving On .... \n \n Details: {0}",ex.ToString());
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void PersistSeed()
        {
            if (!_options.Seed)
            {
                Console.WriteLine("Seeding is not elected. Skipping");
                return;
            }
            TypeLookup typeLookup = new();
            var seeders = typeLookup.GetSeedersByContextName(_options.Context).ToList();
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
