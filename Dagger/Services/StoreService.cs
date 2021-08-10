using System;
using System.Collections.Generic;
using System.Linq;
using Dagger.Models;

namespace Dagger.Services
{
    /// <summary>
    /// Keep track of objects as they are created through runtime and provide methods to use those items.
    /// </summary>
    public static class StoreService
    {
        public static List<Collection> Collections { get; } = new();
        public static List<Writable> Writable { get; } = new();

        /// <summary>
        /// Clear all items from memory.
        /// </summary>
        public static void Clear()
        {
            Collections.Clear();
            Writable.Clear();
        }

        /// <summary>
        /// Compile all collection objects in memory into a single object that can be injected into a template.
        /// </summary>
        /// <returns>
        /// A dictionary where each key is the name of a collection and refers
        /// to a list of items from that collection.
        /// </returns>
        public static Dictionary<string, List<Injectable>> GetInjectables()
        {
            Dictionary<string, List<Injectable>> injectables = new();
            foreach (Collection collection in Collections)
            {
                List<Injectable> injectableList = new();
                injectables.Add(collection.Info.Name, injectableList);
                // injectableList.AddRange(collection.Items.Select(item => item.Serialize()));

                foreach (Resource resource in collection.Items)
                {
                    dynamic injectable = resource.Serialize();
                    injectableList.Add(injectable);
                    Console.WriteLine(injectable.Title);
                }
            }
            return injectables;
        }
    }
}