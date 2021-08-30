using System;
using System.IO;
using Stein.Metadata;
using Stein.Models;
using Stein.Resources;
using Stein.Services;
using HandlebarsDotNet;
using System.Threading;

namespace Stein.Routines
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
            Store store = new();
            
            DirectoryInfo projectInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

            // Todo: Auto-detect templating engine based on file extension.
            const string templateExtension = "hbs";
            
            // Register partials.
            RegisterHandlebarsPartials(
                Directory.GetFiles(PathService.PartialsPath, $"*.{templateExtension}")
                );
            
            foreach (string directoryPath in Directory.GetDirectories(PathService.CollectionsPath))
            {
                // Register the collection.
                DirectoryInfo collectionInfo = new(directoryPath);
                Collection collection = new(collectionInfo);
                
                // Claim files.
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
            
                    // Skip unsupported formats.
                    if (metadata == null) continue;

                    collection.Items.Add(metadata);

                    metadata.Process();

                    // If the resource has not defined a template, no further action to develop a Writable is needed.
                    if (metadata.Template == null) continue;

                    string rawTemplate = null;

                    try
                    {
                        rawTemplate =
                            File.ReadAllText(Path.Join(PathService.TemplatesPath, metadata.Template + ".hbs"));
                    }
                    catch (FileNotFoundException)
                    {
                        metadata.Invalidate(Resource.InvalidType.TemplateNotFound);
                        MessageService.Log(new Message($"Skipped collection item with missing template: {metadata.Info.FullName}",
                                                        Message.InfoType.Warning));
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(20);
                        rawTemplate =
                            File.ReadAllText(Path.Join(PathService.TemplatesPath, metadata.Template + ".hbs"));
                    }
                        
                    if (metadata.IsInvalid) continue;
                        
                    Injectable injectable = metadata.Serialize();

                    var compiledTemplate = Handlebars.Compile(rawTemplate);
                    string renderedTemplate = compiledTemplate(injectable);

                    Writable writable = new(metadata.Info, renderedTemplate);
                    store.Writable.Add(writable);
                }
                
                store.Collections.Add(collection);
            }

            // Assemble an Injectable.
            var injectables = store.GetInjectables();
            
            foreach (string filePath in Directory.GetFiles(PathService.PagesPath, $"*.{templateExtension}"))
            {
                FileInfo pageInfo = new(filePath);
                string rawFile;

                try
                {
                    rawFile = File.ReadAllText(pageInfo.FullName);
                }
                catch (IOException)
                {
                    Thread.Sleep(20);
                    rawFile = File.ReadAllText(pageInfo.FullName);
                }
                
                // Todo: try/catch to handle templates with asymmetrical tags.
                HandlebarsTemplate<object,object> compiledTemplate = Handlebars.Compile(rawFile);
                var renderedTemplate = compiledTemplate(injectables);
                
                Writable writable = new(pageInfo, renderedTemplate);
                store.Writable.Add(writable);
            }
            
            // Todo: Automatic archiving of old versions.
            // Todo: Incremental builds.
            if (Directory.Exists(PathService.SitePath)) Directory.Delete(PathService.SitePath, true);
            
            // Assert public files are up-to-date.
            PathService.Synchronize(
                PathService.ResourcesPublicPath, 
                PathService.SitePublicPath, 
                true
                );
            
            // Finally, writing everything out to file system.
            foreach (Writable writable in store.Writable)
            {
                string directory = Path.GetDirectoryName(writable.Target);
                Directory.CreateDirectory(directory);
                File.WriteAllText(writable.Target, writable.Payload);
            }

            StringService.Colorize($"Built project ", ConsoleColor.Green, false);
            StringService.Colorize($"'{projectInfo.Name}'", ConsoleColor.Gray, true);

            MessageService.Print();
        }

        /// <summary>
        /// Make Handlebars aware of a given partial. The name of the partial will be equal to the file name.
        /// </summary>
        /// <param name="filePath">
        /// The path to the Handlebars partial.
        /// </param>
        private void RegisterHandlebarsPartials(string filePath)
        {
            // Todo: try/catch for missing template partials.
            string template;
            string templateName;

            try
            {
                template = File.ReadAllText(filePath);
                templateName = Path.GetFileNameWithoutExtension(filePath);
            }
            catch (IOException)
            {
                Thread.Sleep(50);
                template = File.ReadAllText(filePath);
                templateName = Path.GetFileNameWithoutExtension(filePath);
            }

            Handlebars.RegisterTemplate(templateName, template);
        }

        /// <summary>
        /// Make Handlebars aware of all given partials. The names of the partials will be equal to the file names.
        /// </summary>
        /// <param name="filePaths">
        /// An array of strings that represent paths to Handlebars partials.
        /// </param>
        private void RegisterHandlebarsPartials(string[] filePaths)
        {
            foreach(string path in filePaths) RegisterHandlebarsPartials(path);
        }
    }
}