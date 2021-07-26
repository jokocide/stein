using System;
using System.Collections.Generic;
using System.IO;
using Dagger.Data.Models;
using HandlebarsDotNet;
using Markdig;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Compile a Dagger project and write the results to the filesystem by invoking methods from the Author service.
    /// </summary>
    public class Build : Routine
    {
        public override void Execute()
        {
            // Pipeline ensures that we are in a Dagger project before returning a Build routine.
            string projectPath = Directory.GetCurrentDirectory();
            
            string resourcesPath = Path.Join(projectPath, "resources");
            string collectionsPath = Path.Join(resourcesPath, "collections");
            string pagesPath = Path.Join(resourcesPath, "pages");
            string publicPath = Path.Join(resourcesPath, "public");
            string templatesPath = Path.Join(resourcesPath, "templates");
            string siteDirectory = Path.Join(projectPath, "site");
            
            string[] partialsPaths = GetHandlebarsPartialsPaths(projectPath);
            RegisterHandlebarsPartials(partialsPaths);

            string[] collectionsDirectoriesPaths = GetCollectionsDirectoriesPaths(projectPath);
            List<string> collectionsMarkdownFilesPaths = GetCollectionMarkdownPaths(collectionsDirectoriesPaths);

            foreach (string path in collectionsMarkdownFilesPaths)
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
        public string[] GetHandlebarsPartialsPaths(string projectPath)
        {
            if (!Helper.CheckIsProject(projectPath))
                throw new InvalidOperationException("Did not receive a valid project path.");
            
            string partialsDirectory = Path.Join(projectPath, "resources", "templates", "partials");
            string[] partialsFiles = Directory.GetFiles(partialsDirectory, "*.hbs");
            return partialsFiles;
        }

        public void RegisterHandlebarsPartials(string filePath)
        {
            string template = File.ReadAllText(filePath);
            string templateName = Path.GetFileNameWithoutExtension(filePath);
            Handlebars.RegisterTemplate(templateName, template);
        }

        public void RegisterHandlebarsPartials(string[] filePaths)
        {
            foreach(string path in filePaths) RegisterHandlebarsPartials(path);
        }
        
        public string[] GetCollectionsDirectoriesPaths(string projectPath)
        {
            if (!Helper.CheckIsProject(projectPath))
                throw new InvalidOperationException("Did not receive a valid project path.");
            
            string collectionsDirectory = Path.Join(projectPath, "resources", "collections");
            return Directory.GetDirectories(collectionsDirectory);
        }

        public string[] GetCollectionMarkdownPaths(string collectionsDirectoryPath)
        {
            return Directory.GetFiles(collectionsDirectoryPath, "*.md");
        }

        public List<string> GetCollectionMarkdownPaths(string[] collectionsDirectoriesPaths)
        {
            List<string> paths = new List<string>();
            
            foreach (string path in collectionsDirectoriesPaths)
            {
                string[] newPaths = GetCollectionMarkdownPaths(path);
                foreach (string newPath in newPaths) paths.Add(newPath);
            }

            return paths;
        }
    }
}