using System;
using System.Collections.Generic;
using System.IO;
using Dagger.Metadata;
using Dagger.Models;
using Dagger.Services;
using HandlebarsDotNet;

namespace Dagger.Routines
{
    /// <summary>
    /// Compile a Dagger project.
    /// </summary>A
    public class BuildRoutine : Routine 
    {   
        public override void Execute()
        {
            DirectoryInfo projectInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            
            StringService.Colorize(ConsoleColor.Cyan, $"Building project ", false);
            StringService.Colorize(ConsoleColor.DarkGray, $"'{projectInfo.Name}'");

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
                    Metadata.Metadata metadata = file.Extension switch
                    {
                        ".md" => new MarkdownMetadata(file),
                        ".csv" => new CsvMetadata(file),
                        ".json" => new JsonMetadata(file),
                        ".toml" => new TomlMetadata(file),
                        ".xml" => new XmlMetadata(file),
                        _ => null
                    };
            
                    // Skip unsupported files.
                    if (metadata == null) break;
                    
                    collection.Items.Add(metadata);
                    
                    metadata.Process();
                }
            }
            
            /*
             * Middleware
             */
            
            // Retrieve an Injectable.
            Injectable injectable = StoreService.Assemble();
            
            
            foreach (string filePath in Directory.GetFiles(PathService.PagesPath, $"*.{templateExtension}"))
            {
                FileInfo pageInfo = new(filePath);
                string rawFile = File.ReadAllText(pageInfo.FullName);
                
                HandlebarsTemplate<object,object> compiledTemplate = Handlebars.Compile(rawFile);
                var renderedTemplate = compiledTemplate(injectable);
                
                Writable writable = new(pageInfo, renderedTemplate);
                StoreService.Writable.Add(writable);
            }
            
            if (Directory.Exists(PathService.SitePath))
            {
                Directory.Delete(PathService.SitePath, true);
            }
            
            PathService.Synchronize(
                PathService.ResourcesPublicPath, 
                PathService.SitePublicPath, 
                true
                );
            
            foreach (Writable writable in StoreService.Writable)
            {
                string directory = Path.GetDirectoryName(writable.Target);
                Directory.CreateDirectory(directory);
                File.WriteAllText(writable.Target, writable.Payload);
            }

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
        /// Make Handlebars aware of all given partials. The name of the partials will be equal to the file names.
        /// </summary>
        /// <param name="filePaths">An array of strings that represent paths to Handlebars partials.</param>
        private void RegisterHandlebarsPartials(string[] filePaths)
        {
            foreach(string path in filePaths) RegisterHandlebarsPartials(path);
        }
    }
}