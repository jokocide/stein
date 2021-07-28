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
            DirectoryInfo assembly = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location));
            DirectoryInfo netVersion = new DirectoryInfo(assembly.Parent.ToString());
            DirectoryInfo tools = new DirectoryInfo(netVersion.Parent.ToString());
            DirectoryInfo version = new DirectoryInfo(tools.Parent.ToString());
            DirectoryInfo content = new DirectoryInfo(Path.Join(version.ToString(), "content"));
            DirectoryInfo example = new DirectoryInfo(Path.Join(content.ToString(), "example"));
            string[] dirs = Directory.GetDirectories(example.FullName);
            
            string[] paths = 
            {
                Path.Join("resources", "collections", "posts"),
                Path.Join("resources", "pages"),
                Path.Join("resources", "public", "css"),
                Path.Join("resources", "templates", "partials"),
                Path.Join("site", "collections", "posts"),
                Path.Join("site", "public", "css")
            };

            foreach (string path in paths) Directory.CreateDirectory(path);

            File.Create(".dagger");
            File.SetAttributes(".dagger", FileAttributes.Hidden);
            
            Console.WriteLine($"Created Dagger project at: {Directory.GetCurrentDirectory()}");
        }
    }
}