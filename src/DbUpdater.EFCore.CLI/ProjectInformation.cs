namespace DbUpdater.EFCore.CLI
{
    internal class ProjectInformation
    {
        public void ShowWelcomeSplash()
        {
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"DbUpdater for EFCore version {GetType().Assembly.GetName().Version}");
            Console.ForegroundColor = currentColor;
            ShowVerbose();
        }

        public static void ShowHelp()
        {
            var instructions = new List<string>
            {
                "Options:",
                "-m|migrate        Migrates the entities. Requires the type option to be specified",
                "-s|seed           Invokes all seed logic in the DbContext assembly",
                "-S|scripts        A comma delimted list of SQL files to execute in the during the migration. Files must be present and added to the deployed artifact",
                "-t|type           The full type of the DbContext to be migrated. Usually, this will extend the DbContext class",
                "",
                "USAGE",
                "Run DLL           dotnet {your_assembly.dll} -m -s --type=MigrationContext --scripts=scriptfolder/script.sql,script.sql",
                "Run executable    {your_assembly.exe} -m -s --type=MigrationContext --scripts=scriptfolder/script.sql,script.sql",
            };
            foreach (var instruction in instructions)
            {
                Console.WriteLine($"{instruction}");
            }
        }

        public static void ShowVerbose()
        {
            var verbosities = new List<string>
            {
                "-h|--help         Display help"
            };
            foreach (var verbosity in verbosities)
            {
                Console.WriteLine($"{verbosity}");
            }
        }
    }
}
