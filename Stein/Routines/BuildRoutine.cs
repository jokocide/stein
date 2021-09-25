using System;
using System.IO;
using Stein.Models;
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
            engine.RegisterPartial(PathService.PartialsFiles);

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
                // We now have a string instead of a FileInfo, so we aren't filtering for "*.hbs" explicitly.
                // we need to make sure the methods implemented on the engine is smart enough to know that
                // it can only handle its own file type.
                string rawFile = PathService.ReadAllSafe(info);

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
    }
}