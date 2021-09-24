using System;
using System.IO;
using Stein.Models;
using Stein.Collections;
using HandlebarsDotNet;
using Stein.Interfaces;
using Stein.Engines;
using Stein.Services;

namespace Stein.Routines
{
    public sealed class BuildRoutine : Routine, IExecutable
    {
        public BuildRoutine(Store store, Configuration config) : base(store, config) { }
        
        public static BuildRoutine GetDefault => new BuildRoutine(new Store(), new ConfigurationService().GetConfigOrNew());

        public void Execute()
        {
            IEngine engine = Engine.GetEngine(Config);
            engine.ClaimPartials(PathService.PartialsPath);

            // This should be implemented on Collection class because these files
            // are independent of the Engine type.
            foreach (string path in PathService.CollectionsDirectories)
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

                    collection.Items.Add(item);
                    Writable writable = Writable.GetWritable(item);

                    if (writable == null) 
                        continue;

                    Store.Writable.Add(writable);
                }

                Store.Collections.Add(collection);
            }

            Injectable injectable = Injectable.Assemble(Store, Config);

            // Implemented on Engine because this depends on the type of engine
            // being used.
            foreach (string info in PathService.PagesFiles)
            {
                // Todo:
                // We now have a string instead of a FileInfo.
                // We need to filter out files that don't match the configuration
                // selected template engine's extension.
                string rawFile = PathService.ReadAllSafe(info);

                //using (var stream = File.Open(info.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                //{
                //    var reader = new StreamReader(stream);
                //    rawFile = reader.ReadToEnd();
                //}

                HandlebarsTemplate<object, object> compiledTemplate = Handlebars.Compile(rawFile);

                var renderedTemplate = compiledTemplate(injectable.Items);

                Writable writable = new(info, renderedTemplate);
                Store.Writable.Add(writable);
            }

            if (Directory.Exists(PathService.SitePath)) Directory.Delete(PathService.SitePath, true);

            PathService.Synchronize(
                PathService.ResourcesPublicPath,
                PathService.SitePublicPath,
                true
                );

            // Implemented on Writable class.
            foreach (Writable writable in Store.Writable)
            {
                string directory = Path.GetDirectoryName(writable.Target);
                Directory.CreateDirectory(directory);
                File.WriteAllText(writable.Target, writable.Payload);
            }

            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Built project ", ConsoleColor.White, false);
            StringService.Colorize($"'{new DirectoryInfo(Directory.GetCurrentDirectory()).Name}' ", ConsoleColor.Gray, true);

            MessageService.Print();
        }

        // private void RegisterHandlebarsPartials(string filePath)
        // {
        //     string templateName = Path.GetFileNameWithoutExtension(filePath);
        //     string template;

        //     using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        //     {
        //         var reader = new StreamReader(stream);
        //         template = reader.ReadToEnd();
        //     }

        //     Handlebars.RegisterTemplate(templateName, template);
        // }

        // private void RegisterHandlebarsPartials(string[] filePaths)
        // {
        //     foreach (string path in filePaths) RegisterHandlebarsPartials(path);
        // }
    }
}