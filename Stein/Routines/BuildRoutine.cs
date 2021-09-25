using System;
using System.IO;
using Stein.Models;
using Stein.Interfaces;
using Stein.Services;
using System.Collections.Generic;

namespace Stein.Routines
{
    public sealed class BuildRoutine : Routine, IExecutable
    {
        public BuildRoutine(Store store, Configuration config) : base(store, config) { }
        
        public static BuildRoutine GetDefault => new BuildRoutine(new Store(), new ConfigurationService().GetConfigOrNew());

        public void Execute()
        {
            IEngine engine = Engine.GetEngine(Config);

            // All other forms of generated content likely rely on partials, so we handle those first.
            engine.RegisterPartial(PathService.PartialsFiles);

            // Collections are often iterated over on other templates, so we build those second.
            IEnumerable<Collection> collections = Collection.GetCollection(PathService.CollectionsDirectories);
            Store.Register(collections);

            // We serialize the collections we've gathered into a format that is suitable for template injection.
            Injectable injectable = Injectable.Assemble(Store, Config);

            // Lastly, compile and render the remaining templates and inject the serialized data.
            IEnumerable<IRenderer> templates = engine.CompileTemplate(PathService.PagesFiles);

            Store.Register(engine.RenderTemplate(templates, injectable));
            Store.Register(Writable.GetWritable(Store.Collections));

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