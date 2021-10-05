using System;
using System.IO;
using Stein.Models;
using Stein.Interfaces;
using Stein.Services;

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
            // Most templates rely on partials, so assemble them first.
            foreach (string path in PathService.PartialsFiles)
                Engine.RegisterPartial(path);

            // Populate the store with collection data that can be used to generate an Injectable, and
            // register each collection's items as a Writable while we are at it.
            foreach (string path in PathService.CollectionsDirectories)
            {
                Collection collection = new(path);
                Store.Register(collection);

                foreach (Item item in collection.Items)
                {
                    Writable writable = Writable.GetWritable(item);

                    if (writable == null)
                        continue;

                    Store.Register(writable);
                }
            }

            // This Injectable object represents the result of serializing all collection
            // items together as dynamic objects, this is what provides template files with 
            // access to the iterable collections and data in stein.json.
            Injectable injectable = Injectable.Assemble(Store, Config);

            // With the Injectable in hand we can render the page files.
            foreach (string path in PathService.PagesFiles)
            {
                Template page = Engine.CompileTemplate(path);

                if (page == null)
                    continue;

                Writable writable = Engine.RenderTemplate(page, injectable);
                Store.Register(writable);
            }

            // Cleaning up the old site directory.
            if (Directory.Exists(PathService.SitePath))
                Directory.Delete(PathService.SitePath, true);

            // Resynchronizing the static directory.
            PathService.Synchronize(
                PathService.ResourcesStaticPath,
                PathService.SiteStaticPath,
                true);

            // Writing the results out the project's site directory.
            foreach (Writable writable in Store.Writable)
            {
                Writable.Write(writable);
            }

            StringService.Colorize($"({DateTime.Now:T}) ", ConsoleColor.Gray, false);
            StringService.Colorize($"Built project ", ConsoleColor.White, false);
            StringService.Colorize($"'{new DirectoryInfo(Directory.GetCurrentDirectory()).Name}' ", ConsoleColor.Gray, true);
        }

        private IEngine Engine { get; }

        private Store Store { get; } = new();

        private Configuration Config { get; }
    }
}