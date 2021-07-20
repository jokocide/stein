using System;
using System.Collections.Generic;
using System.IO;
using Dagger.Data.Models;
using HandlebarsDotNet;
using Markdig;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Attempt to compile the Dagger project that exists in the current directory.
    /// </summary>
    public class Build : Routine
    {
        public override void Execute()
        {
            Store store = new Store();
            
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
                
                // Split the metadata string up by newline characters.
                string[] splitMetadata = metadata.Split(Environment.NewLine);
                
                // Convert each line of the metadata into actual MetaData objects.
                Dictionary<string, string> newMetaData = new Dictionary<string, string>();
                
                foreach (string line in splitMetadata)
                {
                    string[] splitLines = line.Split(":", 2);

                    string key = splitLines[0].Trim();
                    string value = splitLines[1].Trim();

                    newMetaData.Add(key, value);
                }
                
                // Add to store.
                store.Posts.Add(newMetaData);

                // Body becomes HTML.
                string htmlBody = Markdown.ToHtml(body);
                
                // Body is added to newMetaData so we can inject the dictionary as a whole into the template.
                newMetaData.Add("body", htmlBody);
                
                // Load the template.
                string requestedTemplateName = newMetaData["template"];
                string templatesDirectory = Path.Join(Directory.GetCurrentDirectory(), "resources", "templates");
                string pathToTemplate = Path.Join(templatesDirectory, requestedTemplateName + ".hbs");

                // Compile template.
                string templateContent = File.ReadAllText(pathToTemplate);
                var template = Handlebars.Compile(templateContent);

                // Render template.
                string renderedTemplate = template(newMetaData);

                // Add new writable to Store.
                string relative = Path.GetRelativePath("./resources", path);
                Console.WriteLine($"Relative path being passed to writable constructor by a post file: {relative}");
                store.Writable.Add(new Writable(relative, renderedTemplate));
            }
            
            string pagesDirectory = Path.Join(Directory.GetCurrentDirectory(), "resources", "pages");
            string[] pagesFiles = Directory.GetFiles(pagesDirectory, "*.hbs");
            
            foreach (string path in pagesFiles)
            {
                // Read file to memory
                string content = File.ReadAllText(path);
                
                // Run to handlebars
                var template = Handlebars.Compile(content);
            
                // Create writable
                string compiledContent = template(store.Posts);
                string relative = Path.GetRelativePath("./resources", path);
                Console.WriteLine($"Relative path being passed to writable constructor by a page file: {relative}");
                store.Writable.Add(new Writable(relative, compiledContent));
            }
            
            // Clean site directory.
            Directory.Delete("./site", true);

            // Pass all writable objects to Author service.
            Author author = new Author(store.Writable);
            author.Write();
            
            // Synchronize public directories.
            Helper.Synchronize(Path.Join("resources", "public"), Path.Join("site", "public"), true);
        }
    }
}