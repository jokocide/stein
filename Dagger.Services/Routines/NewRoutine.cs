using System;
using System.IO;
using System.Reflection;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Create a new Dagger project.
    /// </summary>
    public class NewRoutine : Routine
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
            
            Helper.Synchronize(example.ToString(), projectDirectory, true);
            
            Helper.Colorize(ConsoleColor.Cyan, "Created Dagger project at ", false);
            Helper.Colorize(ConsoleColor.DarkGray, Directory.GetCurrentDirectory());
        }
    }
}