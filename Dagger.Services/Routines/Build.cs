using System;
using System.IO;
using System.Text.RegularExpressions;
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
                string metadata = content.Substring(firstIndicatorEndIndex, secondIndicatorStartIndex - firstIndicatorEndIndex).Trim(); // MetaData object -> Posts list.
                string body = content.Substring(secondIndicatorStartIndex + 3).Trim(); // Writable object (need to calculate path)
                
                // Convert the metadata string into multiple new MetaData objects.
                string[] splitMetadata = metadata.Split(Environment.NewLine);
                
                foreach (string line in splitMetadata)
                {
                    string[] splitLines = line.Split(":", 2);
                    
                    string key = splitLines[0].Trim();
                    string value = splitLines[1].Trim();

                    if (value.StartsWith('"') && value.EndsWith('"'))
                    {
                        Console.WriteLine($"Before: {value}");
                        value = value.Trim('"');
                        Console.WriteLine($"After: {value}");
                    }
                }
            }
        }
    }
}