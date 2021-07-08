using System;
using System.IO;
using HandlebarsDotNet;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Attempt to compile the Dagger project that exists in the current directory.
    /// </summary>
    public class Build : Routine
    {
        public override void Execute()
        {
            // Find partial files.
            string partialsDirectory = Path.Join(Directory.GetCurrentDirectory(), "resources", "templates", "partials");
            string[] partialsFiles = Directory.GetFiles(partialsDirectory, "*.hbs");
            
            // Register partials with Handlebars.
            foreach (string path in partialsFiles)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                string content = File.ReadAllText(path);
                Handlebars.RegisterTemplate(name, content);
            }
        }
    }
}