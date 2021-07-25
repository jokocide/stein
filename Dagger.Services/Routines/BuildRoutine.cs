using System;
using System.Collections.Generic;
using System.IO;
using Dagger.Data.Models;
using HandlebarsDotNet;
using Markdig;
using Microsoft.VisualBasic.FileIO;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Compile a Dagger project and write the results to the filesystem by invoking methods from the Author service.
    /// </summary>
    public class Build : Routine
    {

        public override void Execute()
        {
            string[] partialsFiles = GetPartials();
            
            // Register partials with Handlebars.
            foreach (string path in partialsFiles)
            {
                // File is loaded into memory.
                string content = File.ReadAllText(path);
                
                // The partial is registered with the name of the file.
                string name = Path.GetFileNameWithoutExtension(path);
                
                Handlebars.RegisterTemplate(name, content);
            }

            // Store collection files as they are discovered.
            List<string> collectionFiles = new List<string>();
            
            string collectionsDirectory = Path.Join(Directory.GetCurrentDirectory(), "resources", "collections");
            string[] directoriesInCollections = Directory.GetDirectories(collectionsDirectory);

            foreach (string directory in directoriesInCollections)
            {
                string[] files = Directory.GetFiles(directory, "*.md");
                
                foreach(string file in files) 
                    collectionFiles.Add(file);
            }

            foreach (string path in collectionFiles)
            {
                // File is loaded into memory.
                string content = File.ReadAllText(path);
                
                // We need to directory and file names to create metadata objects.
                // string directoryName = Path.GetDirectoryName(path);
                // string fileName = Path.GetFileNameWithoutExtension(path);

                DirectoryInfo info = new DirectoryInfo(path);
                
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
                
                // Adding metadata from the file itself.
                foreach (string line in splitMetadata)
                {
                    string[] splitLines = line.Split(":", 2);

                    string key = splitLines[0].Trim();
                    string value = splitLines[1].Trim();

                    newMetaData.Add(key, value);
                }

                // Inject a 'path' that can be used for navigation. todo: fix
                newMetaData.Add("path", Path.Join("collections", info.Parent.Name, Path.GetFileNameWithoutExtension(info.Name), "index.html")); 
                
                // Add to store.
                Store.Posts.Add(newMetaData);

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
                Store.Writable.Add(new Writable(relative, renderedTemplate));
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
                string compiledContent = template(Store.Posts);
                string relative = Path.GetRelativePath("./resources", path);
                Store.Writable.Add(new Writable(relative, compiledContent));
            }
            
            // Clean site directory.
            Directory.Delete("./site", true);

            // Pass all writable objects to Author service.
            Author author = new Author(Store.Writable);
            author.Write();
            
            // Synchronize public directories.
            Helper.Synchronize(Path.Join("resources", "public"), Path.Join("site", "public"), true);
        }
        
        /// <summary>
        /// Return all partial files that can be found within the Dagger project at the given path, or at the
        /// current directory when no path is given.
        /// </summary>
        /// <returns>An array of strings that represent absolute file paths to Handlebars partial files.</returns>
        public string[] GetPartials(string path = null)
        {
            string error = "Unable to return partials, invalid directory.";
            if (path == null) path = Directory.GetCurrentDirectory();
            if (!Helper.CheckIsProject(path)) throw new InvalidOperationException(error);
            string partialsDirectory = Path.Join(path, "resources", "templates", "partials");
            string[] partialsFiles = Directory.GetFiles(partialsDirectory, "*.hbs");
            return partialsFiles;
        }
    }
}