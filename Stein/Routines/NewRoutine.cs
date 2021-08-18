using System;
using System.IO;
using System.Reflection;
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
        /// Create a new project.
        /// </summary>
        public override void Execute()
        {
            string projectDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo assembly = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
            DirectoryInfo netVersion = new DirectoryInfo(assembly.Parent.ToString());
            DirectoryInfo tools = new DirectoryInfo(netVersion.Parent.ToString());
            DirectoryInfo version = new DirectoryInfo(tools.Parent.ToString());
            DirectoryInfo content = new DirectoryInfo(Path.Join(version.ToString(), "content"));
            DirectoryInfo example = new DirectoryInfo(Path.Join(content.ToString(), "example"));

            File.Create(".stein");
            File.SetAttributes(".stein", FileAttributes.Hidden);
            
            PathService.Synchronize(example.ToString(), projectDirectory, true);
            
            StringService.Colorize("Created project ", ConsoleColor.Green, false);
            StringService.Colorize(Directory.GetCurrentDirectory(), ConsoleColor.Gray, true);
        }
    }
}