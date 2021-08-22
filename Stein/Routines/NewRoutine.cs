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
            if (File.Exists("stein.json"))
            {
                MessageService.Log(new Message("Project has already been initialized, 'stein.json' exists in target directory.", Message.InfoType.Error));
                MessageService.Print(true);
            }

            File.Create("stein.json");
            Directory.CreateDirectory(Path.Join("resources", "pages"));
            Directory.CreateDirectory(Path.Join("resources", "templates"));
            Directory.CreateDirectory(Path.Join("resources", "collections"));
            Directory.CreateDirectory(Path.Join("resources", "public"));
            Directory.CreateDirectory("site");

            StringService.Colorize("Created project ", ConsoleColor.Green, false);
            StringService.Colorize(Directory.GetCurrentDirectory(), ConsoleColor.Gray, true);
        }
    }
}