using System;
using System.IO;
using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    /// <summary>
    /// Represents a Routine that can be used to scaffold out common directories and
    /// a new configuration file containing default values.
    /// </summary>
    public sealed class NewRoutine : Routine
    {
        /// <summary>
        /// Initializes a new instance of the NewRoutine class with the specified configuration.
        /// </summary>
        /// <param name="serializedConfiguration">
        /// A serialized JSON string representation of the Configuration that should be used to 
        /// generate the project configuration file.
        /// </param>
        public NewRoutine(string serializedConfiguration)
            => SerializedConfiguration = serializedConfiguration;

        /// <summary>
        /// Scaffold out new projects by creating directories and writing out a configuration file.
        /// </summary>
        public override void Execute()
        {
            File.WriteAllText("stein.json", SerializedConfiguration);
            Directory.CreateDirectory(Path.Join("resources", "pages"));
            Directory.CreateDirectory(Path.Join("resources", "templates", "partials"));
            Directory.CreateDirectory(Path.Join("resources", "collections"));
            Directory.CreateDirectory(Path.Join("resources", "public"));
            Directory.CreateDirectory("site");
            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize("Created project ", ConsoleColor.White, false);
            string projectName = Path.GetFileName(Directory.GetCurrentDirectory());
            StringService.Colorize($"'{projectName}'", ConsoleColor.Gray, true);
        }

        private string SerializedConfiguration { get; }
    }
}