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
                // File is loaded into memory.
                string content = File.ReadAllText(path);
                
                // The partial is registered with the name of the file.
                string name = Path.GetFileNameWithoutExtension(path);
                
                Handlebars.RegisterTemplate(name, content);
            }

            string postsDirectory = Path.Join(Directory.GetCurrentDirectory(), "resources", "posts");
            string[] postsFiles = Directory.GetFiles(postsDirectory, "*.md");

            foreach (string path in postsFiles)
            {
                // File is loaded into memory.
                string content = File.ReadAllText(path);
                
                // Looking for metadata indicator, "---" in YAML.
                int firstIndicatorEndIndex = content.IndexOf("---", 0) + 3;
                int secondIndicatorStartIndex = content.IndexOf("---", firstIndicatorEndIndex);
                
                // Separating metadata and content.
                string metadata = content.Substring(firstIndicatorEndIndex, secondIndicatorStartIndex - firstIndicatorEndIndex).Trim();
                string body = content.Substring(secondIndicatorStartIndex + 3).Trim();
                
                // Add the information to a "posts" collection in the data store.
            }
        }
    }
}