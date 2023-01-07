using CommandLine;
using Microsoft.Extensions.Hosting;

namespace DbUpdater.EFCore.CLI
{
    /// <summary>
    /// Default DbUpdater implementation depending on EFCore to persist code first entities
    /// </summary>
    public static class ContextUpdater
    {
        /// <summary>
        /// Static method to perform all the migrations
        /// using the argument tree
        /// </summary>
        /// <param name="host"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void DeployMigrationFromCLI(this IHost host, string[] args)
        {
            if (!args.Any())
            {
                Environment.Exit(0);
            }
            var parsedOptions = Parser.Default.ParseArguments<MigrationOption>(args);
            if (parsedOptions.Errors.Any()) Environment.Exit(0);

            try
            {
                ProjectInformation info = new();
                info.ShowWelcomeSplash();
                if (parsedOptions.Value.Help)
                {
                    ProjectInformation.ShowHelp();
                    Environment.Exit(0);
                    return;
                }
                Console.WriteLine("Starting migration......");
                _ = new DbContextMigration(host, parsedOptions.Value);
                Console.WriteLine($"Migration complete.");
                Environment.Exit(0);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(-1);
            }

        }
    }
}