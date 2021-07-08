using System;
using System.IO;

namespace Dagger.Services.Routines
{
    public class New : Routine
    {
        /// <summary>
        /// Create a new Dagger project.
        /// </summary>
        public override void Execute()
        {
            string[] paths = new string[6]
            {
                Path.Join("resources", "pages"),
                Path.Join("resources", "posts"),
                Path.Join("resources", "public", "css"),
                Path.Join("resources", "templates"),
                Path.Join("site", "posts"),
                Path.Join("site", "public", "css")
            };

            foreach (string path in paths)
            {
                Directory.CreateDirectory(path);
            }

            File.Create(".dagger");
            File.SetAttributes(".dagger", FileAttributes.Hidden);
            
            Console.WriteLine("Project created.");
        }
    }
}