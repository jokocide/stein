using System;
using System.IO;
using Stein.Models;
using Stein.Collections;
using HandlebarsDotNet;
using Stein.Interfaces;
using System.Collections.Generic;
using Stein.Services;

namespace Stein.Routines
{
    public sealed class BuildRoutine : Routine, IExecutable
    {
        public BuildRoutine(Store store, Configuration config) : base(store, config) { }
        
        public static BuildRoutine GetDefault => new BuildRoutine(new Store(), new ConfigurationService().GetConfigOrNew());

        public void Execute()
        {
            // 1. <PARTIALS>
            // Partials/block files must be processed first, since most other types of 
            // resources will rely on these.
            string[] partials = Directory.GetFiles(PathService.PartialsPath, "*.hbs");
            RegisterHandlebarsPartials(partials);

            // 2. <COLLECTIONS>
            // Collection items are processed second, so that they become available for iteration
            // as soon as possible.
            // 
            string[] collectionPaths = Directory.GetDirectories(PathService.CollectionsPath);
            foreach (string path in collectionPaths)
            {
                DirectoryInfo info = new(path);
                Collection collection = new(info);

                foreach (FileInfo file in info.GetFiles())
                {
                    if (file.Extension == "")
                    {
                        MessageService.Log(Message.NoExtension(file));
                        continue;
                    }

                    Item item = Item.GetItem(file);

                    if (item is not MarkdownItem)
                    {
                        MessageService.Log(new Message($"Format unsupported: {item.Info.Name}", Message.InfoType.Error));
                        continue;
                    }

                    Writable writable = item.Process();
                    if (writable != null) Store.Writable.Add(writable);

                    collection.Items.Add(item);
                }

                Store.Collections.Add(collection);
            }

            // 3. <INJECTABLE>
            // An 'injectable' object is formed by serializing all of the in-memory data
            // available at this point. This object is injected into all page files during the 
            // template rendering step.
            Injectable injectable = new();

            SerializedItem configuration = new ConfigurationService().Serialize();

            if (configuration == null)
            {
                MessageService.Log(Message.InvalidJson(new FileInfo("stein.json")));
                MessageService.Print(true);
            }

            Dictionary<string, object> members = configuration.Pairs;

            foreach (KeyValuePair<string, object> pair in members)
                injectable.Items.Add(pair.Key, pair.Value);

            // Collections are serialized and injected.
            foreach (Collection collection in Store.Collections)
            {
                DateService.Sort(collection, DateService.SortMethod.LatestDate);

                List<SerializedItem> serializedMembers = new();
                //collection.Items.ForEach(item => serializedMembers.Add(item.Serialize()));
                collection.Items.ForEach(item =>
                {
                    ISerializer castedItem = item as ISerializer;
                    serializedMembers.Add(castedItem.Serialize());
                });

                injectable.Items.Add(collection.Info.Name, serializedMembers);
            }

            // 4. <PAGE>
            // Page files can now be reliably rendered, now that we have access to
            // the collection items.
            foreach (FileInfo info in new DirectoryInfo(PathService.PagesPath).GetFiles("*.hbs"))
            {
                string rawFile;

                using (var stream = File.Open(info.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var reader = new StreamReader(stream);
                    rawFile = reader.ReadToEnd();
                }

                HandlebarsTemplate<object, object> compiledTemplate = Handlebars.Compile(rawFile);

                var renderedTemplate = compiledTemplate(injectable.Items);

                Writable writable = new(info, renderedTemplate);
                Store.Writable.Add(writable);
            }

            // 5. <CLEAN>
            // Clean the current site directory.
            if (Directory.Exists(PathService.SitePath)) Directory.Delete(PathService.SitePath, true);

            // 6. <PUBLIC>
            // Synchronize public files.
            PathService.Synchronize(
                PathService.ResourcesPublicPath,
                PathService.SitePublicPath,
                true
                );

            // 7. <WRITE>
            // Export Writable objects.
            foreach (Writable writable in Store.Writable)
            {
                string directory = Path.GetDirectoryName(writable.Target);
                Directory.CreateDirectory(directory);
                File.WriteAllText(writable.Target, writable.Payload);
            }

            // 8. <OUTPUT>
            // Provide some output to the user.
            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Built project ", ConsoleColor.White, false);
            StringService.Colorize($"'{new DirectoryInfo(Directory.GetCurrentDirectory()).Name}' ", ConsoleColor.Gray, true);

            MessageService.Print();
        }

        private void ProcessPartials(string[] partialsFiles)
        {
            throw new NotImplementedException();
        }

        private void RegisterHandlebarsPartials(string filePath)
        {
            string templateName = Path.GetFileNameWithoutExtension(filePath);
            string template;

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new StreamReader(stream);
                template = reader.ReadToEnd();
            }

            Handlebars.RegisterTemplate(templateName, template);
        }

        private void RegisterHandlebarsPartials(string[] filePaths)
        {
            foreach (string path in filePaths) RegisterHandlebarsPartials(path);
        }
    }
}