namespace DbUpdater.EFCore.CLI
{
    /// <summary>
    /// Interface for the seedable APIs
    /// </summary>
    public interface IContextSeedable
    {
        /// <summary>
        /// Executes seed logic by looking into the DbContext assembly to fetch all types that
        /// extends the <see cref="AbstractContextSeeder"/> and executes its seed method.
        /// </summary>
        /// <remarks>The <see cref="AbstractContextSeeder"/> Seed method uses the <see cref="IServiceProvider"/> 
        /// argument to allow instance to the DbContext class from the dependency injection tree</remarks>
        public void PersistSeed();
    }
}
