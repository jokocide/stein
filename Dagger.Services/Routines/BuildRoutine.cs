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

            // Separate frontmatter from body -- function
            // Create some kind of 'frontmatter' object -- function
            // Add object to store
            // Process markdown to html -- markdig
            // Create writable object
            foreach (string path in collectionsMarkdownFilesPaths)
            {
                DirectoryInfo info = new DirectoryInfo(path);
                string content = File.ReadAllText(path);
                (int Start, int End) indices = GetYamlFrontmatterIndices(content);
                
                // Separating metadata and content.
                string metadata = content.Substring(indices.Start, indices.End - indices.Start).Trim(); // MetaData object -> Posts list.
                string body = content.Substring(indices.End + 3).Trim(); // Writable object (need to calculate path)
                
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
        /// Find Handlebars files (*.hbs) that exist in the expected partials directory of a Dagger project.
        /// </summary>
        /// <returns>An array of strings representing paths to Handlebars partials.</returns>
        public string[] GetHandlebarsPartialsPaths(string projectPath)
        {
            if (!Helper.CheckIsProject(projectPath))
                throw new InvalidOperationException("Did not receive a valid project path.");
            
            string partialsDirectory = Path.Join(projectPath, "resources", "templates", "partials");
            string[] partialsFiles = Directory.GetFiles(partialsDirectory, "*.hbs");
            return partialsFiles;
        }

        /// <summary>
        /// Make Handlebars aware of a given partial. The name of the partial will be equal to the file name.
        /// </summary>
        /// <param name="filePath">The path to the Handlebars partial.</param>
        public void RegisterHandlebarsPartials(string filePath)
        {
            string template = File.ReadAllText(filePath);
            string templateName = Path.GetFileNameWithoutExtension(filePath);
            Handlebars.RegisterTemplate(templateName, template);
        }

        /// <summary>
        /// Make Handlebars aware of all given partials. The name of the partials will be equal to the file names.
        /// </summary>
        /// <param name="filePaths">An array of strings that represent paths to Handlebars partials.</param>
        public void RegisterHandlebarsPartials(string[] filePaths)
        {
            foreach(string path in filePaths) RegisterHandlebarsPartials(path);
        }
        
        /// <summary>
        /// Find the paths of all subdirectories within a given Dagger project path's expected collections directory.
        /// </summary>
        /// <param name="projectPath">The Dagger project to be searched.</param>
        /// <returns>
        /// An array of strings representing paths to the present collections within the given project.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the given path does not appear to be a Dagger project. (Missing .dagger file?)
        /// </exception>
        public string[] GetCollectionsDirectoriesPaths(string projectPath)
        {
            if (!Helper.CheckIsProject(projectPath))
                throw new InvalidOperationException("Did not receive a valid project path.");
            
            string collectionsDirectory = Path.Join(projectPath, "resources", "collections");
            return Directory.GetDirectories(collectionsDirectory);
        }

        /// <summary>
        /// Find all Markdown files (*.md) that exist within the given collection directory path.
        /// </summary>
        /// <param name="collectionsDirectoryPath">
        /// A string representing a path to the collection that should be searched.
        /// </param>
        /// <returns>A list of strings representing paths to all existing Markdown files.</returns>
        public string[] GetCollectionMarkdownPaths(string collectionsDirectoryPath)
        {
            return Directory.GetFiles(collectionsDirectoryPath, "*.md");
        }

        /// <summary>
        /// Find all Markdown files (*.md) that exist within all given collection directory paths.
        /// </summary>
        /// <param name="collectionsDirectoriesPaths">
        /// An array of strings representing paths to the the collections that should be searched.
        /// </param>
        /// <returns>A list of strings representing paths to all existing Markdown files.</returns>
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

        /// <summary>
        /// Find the indices representing the end of the first YAML frontmatter indicator and the beginning of the
        /// second YAML frontmatter indicator. all indices between these two numbers represent frontmatter.
        /// </summary>
        /// <param name="text">A string containing YAML frontmatter.</param>
        /// <returns>
        /// A tuple representing the beginning and ending indices of the frontmatter content
        /// (without the traditional YAML '---' indicators)
        /// </returns>
        public (int Start, int End) GetYamlFrontmatterIndices(string text)
        {
            int frontmatterStarts = text.IndexOf("---", 0) + 3;
            int frontmatterEnds = text.IndexOf("---", frontmatterStarts);
            return (frontmatterStarts, frontmatterEnds);
        }
    }
}