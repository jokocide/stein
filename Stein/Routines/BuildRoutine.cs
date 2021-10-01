using System;
using System.IO;
using Stein.Models;
using Stein.Interfaces;
using Stein.Services;
using Stein.Engines;

namespace Stein.Routines
{
    public sealed class BuildRoutine : Routine
    {
        public BuildRoutine() 
        {
            Config = new Configuration().GetConfig();

            switch (Config.Engine)
            {
                case "hbs":
                    Engine = new HandlebarsEngine();
                    break;
            }
        }

        public override void Execute()
        {
            // Most templates rely on partials, so assemble them first.
            foreach(string path in PathService.PartialsFiles)
            {
                Engine.RegisterPartial(path);
            }

            // Populate the store will collection data that can be used to generate an Injectable
            // later on, and register each collection's items as a Writable while we are at it.
            foreach(string path in PathService.CollectionsDirectories)
            {
                Collection collection = Collection.GetCollection(path);
                Store.Register(collection);

                foreach(Item item in collection.Items)
                {
                    Writable writable = Writable.GetWritable(item);
                    Store.Register(writable);
                }
            }

            // This Injectable object represents the result of serializing all collection
            // items together as dynamic objects, this is what provides template files with 
            // access to the iterable collections and data in stein.json.
            Injectable injectable = Injectable.Assemble(Store, Config);

            // With the Injectable in hand we can render the page files.
            foreach(string path in PathService.PagesFiles)
            {
                Template page = Engine.CompileTemplate(path);
                Writable writable = Engine.RenderTemplate(page, injectable);
                Store.Register(writable);
            }

            // Deleting the old site directory.
            if (Directory.Exists(PathService.SitePath))
                Directory.Delete(PathService.SitePath, true);

            // Resynchronizing the static directory.
            PathService.Synchronize(
                PathService.ResourcesStaticPath,
                PathService.SiteStaticPath,
                true);

            // Writing out the results.
            foreach(Writable writable in Store.Writable)
            {
                Writable.Write(writable);
            }

            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Built project ", ConsoleColor.White, false);
            StringService.Colorize($"'{new DirectoryInfo(Directory.GetCurrentDirectory()).Name}' ", ConsoleColor.Gray, true);

            Message.Print();
        }

        private IEngine Engine { get; }

        private Store Store { get; } = new();

        private Configuration Config { get; }
    }
}