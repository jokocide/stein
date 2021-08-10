using System;
using System.IO;
using Dagger.Metadata;
using Dagger.Models;
using Dagger.Resources;
using Dagger.Services;
using HandlebarsDotNet;

namespace Dagger.Routines
{
    /// <summary>
    /// Provide a method that can be used to build a project.
    /// </summary>
    public sealed class BuildRoutine : Routine 
    {   
        /// <summary>
        /// Combine all existing and valid collection and page files to produce HTML, and copy public files to
        /// the site directory. This should produce a functional website at /site.
        /// </summary>
        public override void Execute()
        {
            DirectoryInfo projectInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            
            StringService.Colorize(ConsoleColor.Cyan, $"Building project ", false);
            StringService.Colorize(ConsoleColor.DarkGray, $"'{projectInfo.Name}'");

            // This will change later on when I support more templating engines.
            const string templateExtension = "hbs";
            
            // Register partials.
            RegisterHandlebarsPartials(
                Directory.GetFiles(PathService.PartialsPath, $"*.{templateExtension}")
                );
            
            foreach (string directoryPath in Directory.GetDirectories(PathService.CollectionsPath))
            {
                // Registration.
                DirectoryInfo collectionInfo = new(directoryPath);
                Collection collection = new(collectionInfo);
                StoreService.Collections.Add(collection);
                
                // Claiming files.
                foreach (FileInfo file in collectionInfo.GetFiles())
                {
                    Resource metadata = file.Extension switch
                    {
                        ".md" => new MarkdownResource(file),
                        ".csv" => new CsvResource(file),
                        ".json" => new JsonResource(file),
                        ".toml" => new TomlResource(file),
                        ".xml" => new XmlResource(file),
                        _ => null
                    };
            
                    // Skip unsupported files.
                    if (metadata == null) break;
                    
                    collection.Items.Add(metadata);
                    metadata.Process();
                }
            }
            
            // Assemble an Injectable.
            // Todo: Use the injectable here.
            
            foreach (string filePath in Directory.GetFiles(PathService.PagesPath, $"*.{templateExtension}"))
            {
                FileInfo pageInfo = new(filePath);
                string rawFile = File.ReadAllText(pageInfo.FullName);
                
                HandlebarsTemplate<object,object> compiledTemplate = Handlebars.Compile(rawFile);
                var renderedTemplate = compiledTemplate(StoreService.Collections); // Todo: Injectable object goes in here, not Collections.
                
                Writable writable = new(pageInfo, renderedTemplate);
                StoreService.Writable.Add(writable);
            }
            
            // Todo: Automatic archiving of old versions.
            if (Directory.Exists(PathService.SitePath))
            {
                Directory.Delete(PathService.SitePath, true);
            }
            
            // Assert public files are up-to-date.
            PathService.Synchronize(
                PathService.ResourcesPublicPath, 
                PathService.SitePublicPath, 
                true
                );
            
            // Finally, writing everything out to file system.
            foreach (Writable writable in StoreService.Writable)
            {
                string directory = Path.GetDirectoryName(writable.Target);
                Directory.CreateDirectory(directory);
                File.WriteAllText(writable.Target, writable.Payload);
            }

            // Make room for the next build.
            StoreService.Clear();
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
        /// Make Handlebars aware of all given partials. The names of the partials will be equal to the file names.
        /// </summary>
        /// <param name="filePaths">An array of strings that represent paths to Handlebars partials.</param>
        private void RegisterHandlebarsPartials(string[] filePaths)
        {
            foreach(string path in filePaths) RegisterHandlebarsPartials(path);
        }
    }
}