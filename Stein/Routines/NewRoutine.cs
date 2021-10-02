using System;
using System.IO;
using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    /// <summary>
    /// A Routine used to scaffold out new projects.
    /// </summary>
    public sealed class NewRoutine : Routine
    {
        /// <summary>
        /// Create a new project with the provided configuration.
        /// </summary>
        /// <param name="serializedConfiguration">
        /// A serialized JSON string representation of a Configuration object.
        /// </param>
        public NewRoutine(string serializedConfiguration)
            => SerializedConfiguration = serializedConfiguration;

        public override void Execute()
        {
            File.WriteAllText("stein.json", SerializedConfiguration);
            ScaffoldDirectories();
            WriteUserOutput();
        }

        private string SerializedConfiguration { get; }

        private void ScaffoldDirectories()
        {
            Directory.CreateDirectory(Path.Join("resources", "pages"));
            Directory.CreateDirectory(Path.Join("resources", "templates", "partials"));
            Directory.CreateDirectory(Path.Join("resources", "collections"));
            Directory.CreateDirectory(Path.Join("resources", "public"));
            Directory.CreateDirectory("site");
        }

        private void WriteUserOutput()
        {
            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize("Created project ", ConsoleColor.White, false);
            string projectName = Path.GetFileName(Directory.GetCurrentDirectory());
            StringService.Colorize($"'{projectName}'", ConsoleColor.Gray, true);
        }
    }
}