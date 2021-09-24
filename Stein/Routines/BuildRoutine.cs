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
            IEngine engine;

            switch (Config.Engine)
            {
                case "handlebars":
                    engine = new HandlebarsEngine();
                    break;
                default:
                    MessageService.Log(Message.NoEngine());
                    MessageService.Print(true);
                    return;
            }

            foreach (string path in PathService.PartialsFiles)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                string text = PathService.ReadAllSafe(path);

                engine.RegisterPartial(name, text);
            }

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