using System;
using System.IO;
using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    /// <summary>
    /// Provide a method that can be used to generate a new project.
    /// </summary>
    public sealed class NewRoutine : Routine
    {
        /// <summary>
        /// Assert the the current directory is recognizable as a project. 
        /// </summary>
        public override void Execute()
        {
            // If a stein.json already exists we probably don't want to do anything.
            if (File.Exists("stein.json"))
            {
                MessageService.Log(new Message("A stein.json already exists in target directory.", Message.InfoType.Error));
                MessageService.Print(true);
            }

            // Create a stein.json file and populate it with the default configuration.
            File.WriteAllText("stein.json", ConfigurationService.Defaults);

            // Scaffold out some common directories.
            Directory.CreateDirectory(Path.Join("resources", "pages"));
            Directory.CreateDirectory(Path.Join("resources", "templates", "partials"));
            Directory.CreateDirectory(Path.Join("resources", "collections"));
            Directory.CreateDirectory(Path.Join("resources", "public"));
            Directory.CreateDirectory("site");

            // Provide some output.
            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize("Created project ", ConsoleColor.White, false);
            string projectName = Path.GetFileName(Directory.GetCurrentDirectory());
            StringService.Colorize($"'{projectName}'", ConsoleColor.Gray, true);
        }
    }
}