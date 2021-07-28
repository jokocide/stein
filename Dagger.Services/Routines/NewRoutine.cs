using System;
using System.IO;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Create a new Dagger project.
    /// </summary>
    public class NewRoutine : Routine
    {
        public override void Execute()
        {
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