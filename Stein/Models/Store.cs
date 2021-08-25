using System;
using System.Collections.Generic;
using Stein.Models;

namespace Stein.Services
{
    /// <summary>
    /// Keep track of objects as they are created through runtime and provide methods to use those items.
    /// </summary>
    public class Store
    {
        public List<Collection> Collections { get; } = new();
        public List<Writable> Writable { get; } = new();

        /// <summary>
        /// Compile all collection objects in memory into a single object that can be injected into a template.
        /// </summary>
        /// <returns>
        /// A dictionary where each key is the name of a collection and refers
        /// to a list of items from that collection.
        /// </returns>
        public Dictionary<string, List<Injectable>> GetInjectables()
        {
            Dictionary<string, List<Injectable>> injectables = new();
            
            foreach (Collection collection in Collections)
            {
                LatestDateSort(collection);
                
                List<Injectable> injectableList = new();
                injectables.Add(collection.Info.Name, injectableList);

                foreach (Resource item in collection.Items)
                {
                    dynamic injectable = item.Serialize();
                    injectableList.Add(injectable);
                }
            }

            return injectables;
        }

        /// <summary>
        /// Default sorting method that places the latest items toward the top of the list,
        /// earlier items and items without a date are placed toward the bottom.
        /// </summary>
        public void LatestDateSort(Collection collection)
        {
            collection.Items.Sort((a, b) =>
            {
                if (a.Date == null && b.Date == null) return 0;
                else if (a.Date == null) return 1;
                else if (b.Date == null) return -1;

                DateTime parsedA = DateTime.Parse(a.Date);
                DateTime parsedB = DateTime.Parse(b.Date);

                return DateTime.Compare(parsedB, parsedA);
            });
        }
    }
}