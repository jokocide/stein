using System;
using System.IO;
using Stein.Models;
using Stein.Collections;
using HandlebarsDotNet;
using Stein.Interfaces;
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

            Store.Register(Collection.GetCollection(PathService.CollectionsDirectories));
            Store.Register(Writable.GetWritable(Store.Collections));

            // Create pages
            // Create Writable from pages
            // Write out pages

            Injectable injectable = Injectable.Assemble(Store, Config);

            // Implemented on Engine because this will only convert files that are 
            // compatible with the engine type.
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