using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Dagger.Data.Models;
using HandlebarsDotNet;
using Markdig;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Compile a Dagger project.
    /// </summary>
    public class BuildRoutine : Routine
    {
        public override void Execute()
        {
            DirectoryInfo projectDirectoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            
            Helper.Colorize(ConsoleColor.Cyan, $"Building project ", false);
            Helper.Colorize(ConsoleColor.DarkGray, $"'{projectDirectoryInfo.Name}'");
            
            Store store = new Store();
            
            string projectPath = Directory.GetCurrentDirectory();
            string resourcesPath = Path.Join(projectPath, "resources");
            string sitePath = Path.Join(projectPath, "site");
            string resourcesPublicPath = Path.Join(resourcesPath, "public");
            string sitePublicPath = Path.Join(sitePath, "public");
            string templatesPath = Path.Join(resourcesPath, "templates");
            
            string[] partialsPaths = GetHandlebarsPartialsPaths(projectPath);
            string[] collectionsDirectoriesPaths = GetCollectionsDirectoriesPaths(projectPath);
            string[] pagesPaths = GetPagesPaths(projectPath);
            List<string> collectionsMarkdownFilesPaths = GetCollectionMarkdownPaths(collectionsDirectoriesPaths);

            // Handle partials.
            RegisterHandlebarsPartials(partialsPaths);

            // Handle collection files.
            foreach (string filePath in collectionsMarkdownFilesPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                
                // directoryInfo should always have a parent.
                if (directoryInfo.Parent?.Name == null) throw new Exception("Received null parent value.");
                
                string fileContent;
                
                try
                {
                    fileContent = File.ReadAllText(filePath);
                }
                catch (IOException)
                {
                    // File is in use. Wait and try again.
                    Thread.Sleep(200);
                    fileContent = File.ReadAllText(filePath);
                }

                (int FirstStart, int FirstEnd, int SecondStart, int SecondEnd) indices = new();

                // Try to establish indices, but return an error if the frontmatter is abnormal.
                try
                {
                    indices = GetYamlFrontmatterIndices(fileContent);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Message message = new Message()
                    {
                        Text = $"Collection item {directoryInfo.Name} appears to have invalid YAML frontmatter.",
                        Type = Message.MessageType.Error
                    };
                    
                    HelpRoutine help = new HelpRoutine(message);
                    help.Execute();
                }
                
                Dictionary<string, string> metadata = CreateMetadata(
                    Helper.Slice(indices.FirstEnd, indices.SecondStart, fileContent).Trim()
                );
                
                /*
                 * Collection files receive a 'path' key to facilitate creating links to collection files while
                 * iterating in a template. Modify Writable.cs to change the output location of collection files.
                 */
                metadata.Add("path",
                    Path.Join
                    (
                        directoryInfo.Parent.Name,
                        Path.GetFileNameWithoutExtension(directoryInfo.Name), 
                        "index.html"
                    )
                );
                
                // Initialize a new collection for this file, if one doesn't already exist.
                if (!store.Collections.ContainsKey(directoryInfo.Parent.Name))
                    store.Collections.Add(directoryInfo.Parent.Name, new List<Dictionary<string, string>>());
                
                /*
                 * Metadata object is added to the store at this point. Keys added to the dictionary beyond this
                 * point will still be included because this is a reference type, but this is a logical separation.
                 */
                store.Collections[directoryInfo.Parent.Name].Add(metadata);
                
                // Select and convert the body of the file to HTML.
                string untransformedBody = fileContent[indices.SecondEnd..].Trim();
                string transformedBody = Markdown.ToHtml(untransformedBody);
                
                // Add a 'body' key that refers to the newly created HTML body to the metadata object.
                metadata.Add("body", transformedBody);

                /*
                 * Markdown collection files should have a 'template' key. Here we attempt to read the
                 * requested template.
                 */
                string template = null;
                try
                {
                    template = File.ReadAllText(Path.Join(templatesPath, metadata["template"] + ".hbs"));
                }
                catch (FileNotFoundException e)
                {
                    Message message = new Message()
                    {
                        Text = $"Unable to locate the template '{Path.GetFileName(e.FileName)}' while processing " 
                               + $"the '{directoryInfo.Name}' collection item.",
                        Type = Message.MessageType.Error
                    };
                    
                    HelpRoutine help = new HelpRoutine(message);
                    help.Execute();
                }

                HandlebarsTemplate<object,object> compiledTemplate = Handlebars.Compile(template);
                string renderedTemplate = compiledTemplate(metadata);

                // Create a Writable.
                string resourcePath = Path.GetRelativePath("./resources", filePath);
                Writable writable = new Writable(resourcePath, renderedTemplate);

                store.Writable.Add(writable);
            }
            
            // Handle page files.
            foreach (string filePath in pagesPaths)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
                string fileContent = File.ReadAllText(filePath);

                var compiledTemplate = Handlebars.Compile(fileContent);
                var renderedTemplate = compiledTemplate(store.Collections);

                string resourcePath = Path.GetRelativePath("resources", directoryInfo.FullName);
                Writable writable = new Writable(resourcePath, renderedTemplate);
                
                store.Writable.Add(writable);
            }

            // Todo: Maybe we should archive old versions of sites instead of deleting them, or provide the option.
            // Clear the site directory.
            if (Directory.Exists(sitePath)) Directory.Delete(sitePath, true);
            
            // Invoke Author service to handle the Writable objects.
            new Author(store.Writable).Write();
                
            // Synchronize public directories.
            Helper.Synchronize(resourcesPublicPath, sitePublicPath, true);
        }

        /// <summary>
        /// Find Handlebars files (*.hbs) that exist in the expected partials directory of a Dagger project.
        /// </summary>
        /// <param name="projectPath">The Dagger project to be searched.</param>
        /// <returns>An array of strings representing paths to Handlebars partials.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string[] GetHandlebarsPartialsPaths(string projectPath)
        {
            if (!Helper.CheckIsProject(projectPath))
                throw new InvalidOperationException("Did not receive a valid project path.");
            
            string partialsDirectory = Path.Join(projectPath, "resources", "templates", "partials");
            string[] partialsFiles = Directory.GetFiles(partialsDirectory, "*.hbs");
            return partialsFiles;
        }

        /// <summary>
        /// Find Handlebars files (*.hbs) that exist in the expected pages directory of a Dagger project.
        /// </summary>
        /// <param name="projectPath">The Dagger project to be searched.</param>
        /// <returns>An array of strings representing paths to Handlebars partials.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string[] GetPagesPaths(string projectPath)
        {
            if (!Helper.CheckIsProject(projectPath))
                throw new InvalidOperationException("Did not receive a valid project path.");

            string pagesDirectory = Path.Join(projectPath, "resources", "pages");
            string[] pagesFiles = Directory.GetFiles(pagesDirectory, "*.hbs");
            return pagesFiles;
        }

        /// <summary>
        /// Make Handlebars aware of a given partial. The name of the partial will be equal to the file name.
        /// </summary>
        /// <param name="filePath">The path to the Handlebars partial.</param>
        private void RegisterHandlebarsPartials(string filePath)
        {
            string template = File.ReadAllText(filePath);
            string templateName = Path.GetFileNameWithoutExtension(filePath);
            Handlebars.RegisterTemplate(templateName, template);
        }

        /// <summary>
        /// Make Handlebars aware of all given partials. The name of the partials will be equal to the file names.
        /// </summary>
        /// <param name="filePaths">An array of strings that represent paths to Handlebars partials.</param>
        private void RegisterHandlebarsPartials(string[] filePaths)
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
        private string[] GetCollectionsDirectoriesPaths(string projectPath)
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
        private string[] GetCollectionMarkdownPaths(string collectionsDirectoryPath)
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
        private List<string> GetCollectionMarkdownPaths(string[] collectionsDirectoriesPaths)
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
        private (int FirstStart, int FirstEnd, int SecondStart, int SecondEnd) GetYamlFrontmatterIndices(string text)
        {
            int firstStart = text.IndexOf("---", StringComparison.Ordinal);
            int firstEnd = firstStart + 3;
            int secondStart = text.IndexOf("---", firstEnd, StringComparison.Ordinal);
            int secondEnd = secondStart + 3;
            return (firstStart, firstEnd, secondStart, secondEnd);
        }

        /// <summary>
        /// Create a MetaData object by dividing key/value pairs by a specified delimiter, or ':' by
        /// default if no delimiter is given.
        /// </summary>
        /// <param name="text">The source string.</param>
        /// <param name="delimiter">The delimiter that will be used to divide the lines.</param>
        /// <returns>A new MetaData object containing the key/value pairs from the source string.</returns>
        private Dictionary<string, string> CreateMetadata(string text, string delimiter = ":")
        {
            var dictionary = new Dictionary<string, string>();
            string[] lines = text.Split(Environment.NewLine);
            foreach (string line in lines)
            {
                string[] splitLines = line.Split(delimiter, 2);
                string key = splitLines[0].Trim();
                string value = splitLines[1].Trim();
                dictionary.Add(key, value);
            }
            return dictionary;
        }
    }
}