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

            // Collection items are processed first so that Page files have access to iterable data.
            foreach (string directoryPath in Directory.GetDirectories(PathService.CollectionsPath))
            {
                DirectoryInfo collectionInfo = new(directoryPath);

                // The contents of each collection will be stored in a Collection object and
                // then registered with the Store after converting any files inside into Item objects.
                Collection collection = new(collectionInfo);

                // Look at the file extension of each item and create a suitable Item object,
                // which will come with instructions and rules to help pull information from this item.
                foreach (FileInfo info in collectionInfo.GetFiles())
                {
                    // Skip file if it doesn't specify an extension.
                    if (info.Extension == "")
                    {
                        MessageService.Log(Message.NoExtension(info));
                        continue;
                    }

                    // Create Item object.
                    Item item = Item.GetItem(info);

                    // Most of the Item types are not implemented yet, so warn the user if they are trying
                    // to use any of them and continue to the next file.
                    if (item is not MarkdownItem)
                    {
                        MessageService.Log(new Message($"Format unsupported: {item.Info.Name}", Message.InfoType.Error));
                        continue;
                    }

                    // Pull information from the Item and create a Writable if a valid template key is provided.
                    item.Process(store);

                    // Register the Item with the Collection.
                    collection.Items.Add(item);
                }

                // Register the collection with the store.
                store.Collections.Add(collection);
            }

            // Assemble a dynamic object that can be injected into a template.
            var injectables = store.GetInjectables();

            // Page files are processed next. Handlebars is currently the only supported template engine,
            // so we grab everything that ends with '.hbs'.
            foreach (FileInfo info in new DirectoryInfo(PathService.PagesPath).GetFiles("*.hbs"))
            {
                string rawFile;

                // Read the file contents into rawFile.
                using (var stream = File.Open(info.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var reader = new StreamReader(stream);
                    rawFile = reader.ReadToEnd();
                }

                // Each page file is considered its own template, so compile right away.
                HandlebarsTemplate<object, object> compiledTemplate = Handlebars.Compile(rawFile);

                // Render the item with the injectables so that the template has access to all of 
                // the collections.
                var renderedTemplate = compiledTemplate(injectables);

                // Register a new Writable with the Store.
                Writable writable = new(info, renderedTemplate);
                store.Writable.Add(writable);
            }

            // Delete the old site.
            if (Directory.Exists(PathService.SitePath)) Directory.Delete(PathService.SitePath, true);

            // Copy the public files over.
            PathService.Synchronize(
                PathService.ResourcesPublicPath,
                PathService.SitePublicPath,
                true
                );

            // Write out the Writable objects that are registered with the Store.
            foreach (Writable writable in store.Writable)
            {
                string directory = Path.GetDirectoryName(writable.Target);
                Directory.CreateDirectory(directory);
                File.WriteAllText(writable.Target, writable.Payload);
            }

            // Provide user output.
            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Built project ", ConsoleColor.White, false);
            StringService.Colorize($"'{projectInfo.Name}' ", ConsoleColor.Gray, true);

            // Display any warnings/errors.
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