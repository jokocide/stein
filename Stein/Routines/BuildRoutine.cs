using System;
using System.IO;
using Stein.Models;
using Stein.Interfaces;
using Stein.Services;

namespace Stein.Routines
{
    public sealed class BuildRoutine : Routine, IExecutable
    {
        public BuildRoutine(IEngine engine, Store store, Configuration config) : base(engine, store, config) { }
        
        public static BuildRoutine GetDefault()
        {
            Configuration config = new ConfigurationService().GetConfigOrNew();
            IEngine engine = Models.Engine.GetEngine(config);

            return new BuildRoutine(engine, new Store(), config);
        }

        public void Execute()
        {
            // Most templates rely on partials, so assemble them first.
            Engine.RegisterPartial(PathService.PartialsFiles);

            // Page files rely on Collection data, so register them with the store.
            Store.Register(Collection.GetCollection(PathService.CollectionsDirectories));

            // We can serialize the data in Store into dynamic objects and pack them together
            // as an Injectable object.
            Injectable injectable = Injectable.Assemble(Store, Config);

            // With the Collections in hand, we can now safely render the Page files.
            Store.Register(Engine.RenderTemplate(Engine.CompileTemplate(PathService.PagesFiles), injectable));

            // Lastly, we can convert the Collection items into Writable objects as well.
            Store.Register(Writable.GetWritable(Store.Collections));

            if (Directory.Exists(PathService.SitePath))
                Directory.Delete(PathService.SitePath, true);

            PathService.Synchronize(
                PathService.ResourcesPublicPath,
                PathService.SitePublicPath,
                true);

            Writable.Write(Store.Writable);

            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Built project ", ConsoleColor.White, false);
            StringService.Colorize($"'{new DirectoryInfo(Directory.GetCurrentDirectory()).Name}' ", ConsoleColor.Gray, true);

            MessageService.Print();
        }
    }
}