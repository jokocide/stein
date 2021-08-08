using System;
using System.IO;
using System.Reflection;
using Dagger.Models;
using Dagger.Services;

namespace Dagger.Routines
{
    /// <summary>
    /// Create a new Dagger project.
    /// </summary>
    public sealed class NewRoutine : Routine
    {
        public override void Execute()
        {
            string projectDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo assembly = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
            DirectoryInfo netVersion = new DirectoryInfo(assembly.Parent.ToString());
            DirectoryInfo tools = new DirectoryInfo(netVersion.Parent.ToString());
            DirectoryInfo version = new DirectoryInfo(tools.Parent.ToString());
            DirectoryInfo content = new DirectoryInfo(Path.Join(version.ToString(), "content"));
            DirectoryInfo example = new DirectoryInfo(Path.Join(content.ToString(), "example"));

            File.Create(".dagger");
            File.SetAttributes(".dagger", FileAttributes.Hidden);
            
            PathService.Synchronize(example.ToString(), projectDirectory, true);
            
            StringService.Colorize(ConsoleColor.Cyan, "Created Dagger project at ", false);
            StringService.Colorize(ConsoleColor.DarkGray, Directory.GetCurrentDirectory());
        }
    }
}