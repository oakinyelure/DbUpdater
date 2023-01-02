using Microsoft.Extensions.DependencyInjection;

namespace DbUpdater.EFCore.CLI
{
    /// <summary>
    /// An abstract class for all seedable classes. 
    /// The abstract class contains properties to help order seeding steps 
    /// where one seed must complete before another seed.
    /// </summary>
    public abstract class AbstractContextSeeder
    {
        /// <summary>
        /// The order the seed needs to be persisted to 
        /// the database
        /// </summary>
        public abstract int Order { get; set; }

        /// <summary>
        /// Persists the seed data into the database. 
        /// Seed class gets to write their seed logic
        /// </summary>
        /// <param name="provider">Inject the service provider to get instance of your database context</param>
        public abstract void Seed(IServiceScope provider);
    }
}
