using CommandLine;

namespace DbUpdater.EFCore.CLI
{
    /// <summary>
    /// Console options expected when the instance of the dbupdater is ran
    /// </summary>
    public class MigrationOption
    {
        /// <summary>
        /// Set option to true to display help
        /// </summary>
        [Option('h', "help", Required = false, HelpText = "Display help")]
        public bool Help { get; set; }

        /// <summary>
        /// All the SQL scripts to be deployed as part of the migration
        /// </summary>
        [Option('S', "scripts", Required = false, HelpText = "All the SQL scripts to be deployed as part of the migration")]
        public IEnumerable<string> Scripts { get; set; }

        /// <summary>
        /// Set to true if migration needs to be persisted into the database
        /// </summary>
        [Option('m', "migrate", Required = false, HelpText = "Set to true if migration needs to be persisted into the database")]
        public bool Migrate { get; set; }

        /// <summary>
        /// Set to true if seed data needs to be persisted into the database
        /// </summary>
        [Option('s', "seed", Required = false, HelpText = "Set to true if seed data needs to be persisted into the database")]
        public bool Seed { get; set; }

        /// <summary>
        /// Type to get the EF context / DBContext instance from
        /// </summary>
        /// <remarks>Defaults to executing assembly if no value is provided</remarks>
        [Option('t', "type", Required = false, HelpText = "Type to get the EF context / DBContext instance from")]
        public string Type { get; set; }
    }
}
