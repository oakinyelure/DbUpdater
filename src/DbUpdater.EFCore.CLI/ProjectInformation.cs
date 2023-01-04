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
                "-m, --migrate        Set to true if migration needs to be persisted into the database",
                "-s, --seed           Set to true if seed data needs to be persisted into the database",
                "-S, --scripts        All the SQL scripts to be deployed as part of the migration",
                "-c, --context        Full name of the EF context class / DBContext instance",
                "-h, --help           Display help",
                "",
                "USAGE",
                "Run DLL           dotnet {your_assembly.dll} -m -s --type=AssemblyName.NameOfMigrationContext --scripts=scriptfolder/script.sql script.sql",
                "Run executable    {your_assembly.exe} -m -s -t=AssemblyName.NameOfMigrationContext -S=scriptfolder/script.sql script.sql",
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
                "-h, --help         Display help"
            };
            foreach (var verbosity in verbosities)
            {
                Console.WriteLine($"{verbosity}");
            }
        }
    }
}
