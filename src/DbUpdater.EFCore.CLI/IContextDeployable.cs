namespace DbUpdater.EFCore.CLI
{
    /// <summary>
    /// Interface for propalgating entities to the persistence later
    /// </summary>
    public interface IContextDeployable
    {
        /// <summary>
        /// Completes the migration of the code first entities to the database
        /// </summary>
        public void PersistMigration();

        /// <summary>
        /// Deploys Raw SQL scripts to the database using EF Core
        /// </summary>
        public void PersistScript();
    }
}
