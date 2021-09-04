using System;
using System.IO;
using Stein.Models;
using Stein.Collections;
using Stein.Services;
using HandlebarsDotNet;
using System.Threading;

namespace Stein.Routines
{
    /// <summary>Provide a method that can be used to build a project.</summary>
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
            RegisterHandlebarsPartials(Directory.GetFiles(PathService.PartialsPath, "*.hbs"));

            foreach (string directoryPath in Directory.GetDirectories(PathService.CollectionsPath))
            {
                DirectoryInfo collectionInfo = new(directoryPath);
                Collection collection = new(collectionInfo);

                foreach (FileInfo file in collectionInfo.GetFiles())
                {
                    CollectionItem metadata = file.Extension switch
                    {
                        ".md" => new MarkdownItem(file),
                        ".csv" => new CsvItem(file),
                        ".json" => new JsonItem(file),
                        ".toml" => new TomlItem(file),
                        ".xml" => new XmlItem(file),
                        _ => null
                    };

                    if (metadata is not MarkdownItem)
                    {
                        MessageService.Log(new Message($"Markdown is currently the only supported format, skipped: {metadata.Info.Name}", Message.InfoType.Error));
                        continue;
                    }

                    collection.Items.Add(metadata);
                    metadata.Process(store);
                }

                store.Collections.Add(collection);
            }

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
        /// <param name="filePath">
        /// The path to the Handlebars partial.
        /// </param>
        private void RegisterHandlebarsPartials(string filePath)
        {
            string template;
            string templateName;

            try
            {
                template = File.ReadAllText(filePath);
                templateName = Path.GetFileNameWithoutExtension(filePath);
            }
            catch (IOException)
            {
                Thread.Sleep(100);
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
            foreach (string path in filePaths) RegisterHandlebarsPartials(path);
        }
    }
}