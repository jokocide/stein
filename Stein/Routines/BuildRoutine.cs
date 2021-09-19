using System;
using System.IO;
using System.Linq;
using Stein.Models;
using Stein.Collections;
using Stein.Services;
using HandlebarsDotNet;
using Stein.Interfaces;
using System.Collections.Generic;

namespace Stein.Routines
{
    public sealed class BuildRoutine : IExecutable
    {
        public void Execute()
        {
            Store store = new();
            DirectoryInfo projectInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

            // 1. <PARTIALS>
            // Partials/block files must be processed first, since most other types of 
            // resources will rely on these.
            string[] partials = Directory.GetFiles(PathService.PartialsPath, "*.hbs");
            RegisterHandlebarsPartials(partials);

            // 2. <COLLECTIONS>
            // Collection items are processed second, so that they become available for iteration
            // as soon as possible.
            foreach (string directoryPath in Directory.GetDirectories(PathService.CollectionsPath))
            {
                DirectoryInfo collectionInfo = new(directoryPath);
                Collection collection = new(collectionInfo);
                foreach (FileInfo info in collectionInfo.GetFiles())
                {
                    if (info.Extension == "")
                    {
                        MessageService.Log(Message.NoExtension(info));
                        continue;
                    }

                    Item item = Item.GetItem(info);

                    if (item is not MarkdownItem)
                    {
                        MessageService.Log(new Message($"Format unsupported: {item.Info.Name}", Message.InfoType.Error));
                        continue;
                    }

                    if (item is ISerializable castedItem)
                    {
                        item.Process(store);
                        collection.Items.Add(castedItem);
                    }
                }

                store.Collections.Add(collection);
            }

            // 3. <INJECTABLE>
            // An 'injectable' object is formed by serializing all of the in-memory data
            // available at this point. This object is injected into all page files during the 
            // template rendering step.
            Injectable injectable = new();

            SerializedItem serializedConfiguration = new Configuration().Serialize();
            Dictionary<string, object> configurationDictionary = serializedConfiguration.ReturnMembers();
            configurationDictionary.ToList().ForEach(pair => injectable.Items.Add(pair.Key, pair.Value));

            foreach (Collection collection in store.Collections)
            {
                DateService.LatestDateSort(collection);

                SerializedItem serializedCollection = collection.Serialize();
                injectable.Items.Add(collection.)
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

                var renderedTemplate = compiledTemplate(injectables);

                Writable writable = new(info, renderedTemplate);
                store.Writable.Add(writable);
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
            foreach (Writable writable in store.Writable)
            {
                string directory = Path.GetDirectoryName(writable.Target);
                Directory.CreateDirectory(directory);
                File.WriteAllText(writable.Target, writable.Payload);
            }

            // 8. <OUTPUT>
            // Provide some output to the user.
            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Built project ", ConsoleColor.White, false);
            StringService.Colorize($"'{projectInfo.Name}' ", ConsoleColor.Gray, true);

            MessageService.Print();
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