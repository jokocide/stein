using System;
using System.IO;
using Stein.Models;
using Stein.Collections;
using Stein.Services;
using HandlebarsDotNet;

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
            // Each build will have its own Store, which is a data structure that keeps
            // track of important objects that are created while building a project.
            Store store = new();

            // Handlebars is currently the only supported template engine, so immediately
            // register all partials with an extension of '.hbs'.
            RegisterHandlebarsPartials(Directory.GetFiles(PathService.PartialsPath, "*.hbs"));

            DirectoryInfo projectInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

            // Page files usually depend on information that is derived from collection items, 
            // so process the collections first.
            foreach (string directoryPath in Directory.GetDirectories(PathService.CollectionsPath))
            {
                DirectoryInfo collectionInfo = new(directoryPath);

                // The contents of each collection will be stored in a Collection object and
                // then registered with the Store.
                Collection collection = new(collectionInfo);

                // Look at the file extension of each item and create a specific type of Item object,
                // which will come with instructions and rules to help pull information from this item.
                foreach (FileInfo file in collectionInfo.GetFiles())
                {
                    CollectionItem item = file.Extension switch
                    {
                        ".md" => new MarkdownItem(file),
                        ".csv" => new CsvItem(file),
                        ".json" => new JsonItem(file),
                        ".toml" => new TomlItem(file),
                        ".xml" => new XmlItem(file),
                        _ => null
                    };

                    // Most of the Item types are not implemented yet, so warn the user if they are trying
                    // to use any of them.
                    if (item is not MarkdownItem)
                    {
                        MessageService.Log(new Message($"Format unsupported: {item.Info.Name}", Message.InfoType.Error));
                        continue;
                    }

                    // Process(Store) will actually pull information from the file and create a Writable
                    // if a valid Template key is provided.
                    item.Process(store);

                    // Register the item with the collection.
                    collection.Items.Add(item);
                }

                // Register the collection with the store.
                store.Collections.Add(collection);
            }

            // Assemble a dynamic object that can be injected into a template.
            var injectables = store.GetInjectables();

            foreach (string filePath in Directory.GetFiles(PathService.PagesPath, "*.hbs"))
            {
                FileInfo pageInfo = new(filePath);
                string rawFile;
                rawFile = File.ReadAllText(pageInfo.FullName);

                HandlebarsTemplate<object, object> compiledTemplate = Handlebars.Compile(rawFile);
                var renderedTemplate = compiledTemplate(injectables);

                Writable writable = new(pageInfo, renderedTemplate);
                store.Writable.Add(writable);
            }

            if (Directory.Exists(PathService.SitePath)) Directory.Delete(PathService.SitePath, true);

            PathService.Synchronize(
                PathService.ResourcesPublicPath,
                PathService.SitePublicPath,
                true
                );

            foreach (Writable writable in store.Writable)
            {
                string directory = Path.GetDirectoryName(writable.Target);
                Directory.CreateDirectory(directory);
                File.WriteAllText(writable.Target, writable.Payload);
            }

            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Built project ", ConsoleColor.White, false);
            StringService.Colorize($"'{projectInfo.Name}' ", ConsoleColor.Gray, false);

            if (!MessageService.HasError && !MessageService.HasWarning)
            {
                Console.WriteLine();
            }
            else if (MessageService.HasError && MessageService.HasWarning)
            {
                StringService.Colorize("(", ConsoleColor.Gray, false);
                StringService.Colorize($"{MessageService.ErrorCount}", ConsoleColor.Red, false);
                StringService.Colorize(", ", ConsoleColor.Gray, false);
                StringService.Colorize($"{MessageService.WarningCount}", ConsoleColor.Yellow, false);
                StringService.Colorize(")", ConsoleColor.Gray, true);
            }
            else if (MessageService.HasError && !MessageService.HasWarning)
            {
                StringService.Colorize("(", ConsoleColor.Gray, false);
                StringService.Colorize($"{MessageService.ErrorCount}", ConsoleColor.Red, false);
                StringService.Colorize(")", ConsoleColor.Gray, true);
            }
            else if (!MessageService.HasError && MessageService.HasWarning)
            {
                StringService.Colorize("(", ConsoleColor.Gray, false);
                StringService.Colorize($"{MessageService.WarningCount}", ConsoleColor.Yellow, false);
                StringService.Colorize(")", ConsoleColor.Gray, true);
            }

            MessageService.Print();
        }

        /// <summary>
        /// Make Handlebars aware of a given partial. The name of the partial will be equal to the file name.
        /// </summary>
        /// <param name="filePath">The path to the Handlebars partial.</param>
        private void RegisterHandlebarsPartials(string filePath)
        {
            string templateName = Path.GetFileNameWithoutExtension(filePath);
            string template;

            // Reading with ReadWrite on FileAccess & FileShare will prevent IOException during ServeRoutine.
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var reader = new StreamReader(stream);
                template = reader.ReadToEnd();
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
            foreach (string path in filePaths) RegisterHandlebarsPartials(path);
        }
    }
}