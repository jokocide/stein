using System;
using System.IO;
using Stein.Models;
using Stein.Interfaces;
using Stein.Services;
using System.Linq;

namespace Stein.Routines
{
    /// <summary>
    /// Represents a Routine that can be used to build the resources of a project into
    /// a finished site.
    /// </summary>
    public sealed class BuildRoutine : Routine
    {
        /// <summary>
        /// Initializes a new instance of the BuildRoutine class with the provided configuration and engine.
        /// </summary>
        /// <param name="config">A Configuration object used to influence behavior of the routine.</param>
        /// <param name="engine">The desired templating engine.</param>
        public BuildRoutine(Configuration config, IEngine engine)
        {
            Config = config;
            Engine = engine;
        }

        /// <summary>
        /// Transforms the resources of a project into a finished site.
        /// </summary>
        public override void Execute()
        {
            string[] pages = Directory.GetFiles(PathService.GetResourcesPath(), $"*{Config.Engine}", SearchOption.AllDirectories)
                .Where(item => !PathService.IsIgnored(item))
                .ToArray<string>();

            string[] collections = Directory.GetDirectories(PathService.GetResourcesPath(), "", SearchOption.AllDirectories)
                .Where(item => !PathService.IsIgnored(item))
                .ToArray<string>();

            // Most templates rely on partials, so assemble them first.
            string[] partials = Directory.GetFiles(PathService.GetPartialsPath());

            foreach (string path in partials)
                Engine.RegisterPartial(path);

            // Populate the store with collection data that can be used to generate an Injectable, and
            // register each collection's items as a Writable while we are at it.
            foreach (string c in collections)
            {
                Collection collection = new Collection(c);
                Store.Register(collection);

                foreach (Item item in collection.Items)
                {
                    Writable writable = Writable.GetWritable(item, Config, Engine);

                    if (writable == null)
                        continue;

                    Store.Register(writable);
                }
            }



            // This Injectable object represents the result of serializing all collection
            // items together as dynamic objects, this is what provides template files with 
            // access to the iterable collections and data in stein.json.
            Injectable injectable = new Injectable(Store, Config);

            // With the Injectable in hand we can render the page files.
            foreach (string c in pages)
            {
                Template page = Engine.CompileTemplate(c);

                if (page == null)
                    continue;

                string result = Engine.RenderTemplate(page, injectable);
                Writable writable = new Writable(new FileInfo(c), result);

                if (writable == null)
                    continue;

                Store.Register(writable);
            }

            foreach (Writable c in Store.Writable)
            {
                System.Console.WriteLine(c.Target);
            }

            // Cleaning up the old site directory.
            if (Directory.Exists(PathService.GetSitePath()))
                Directory.Delete(PathService.GetSitePath(), true);

            // Resynchronizing the static directory.
            PathService.Synchronize(
                PathService.GetResourcesStaticPath(),
                PathService.GetSiteStaticPath(),
                true);

            // Writing the results out the project's site directory.
            foreach (Writable writable in Store.Writable)
            {
                Writable.Write(writable);
            }
            //
            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Built project ", ConsoleColor.White, false);
            StringService.Colorize($"'{new DirectoryInfo(Directory.GetCurrentDirectory()).Name}' ", ConsoleColor.Gray, true);
        }

        private IEngine Engine { get; }

        private Store Store { get; } = new Store();

        private Configuration Config { get; }
    }
}