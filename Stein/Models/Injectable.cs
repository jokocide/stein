using System.Collections.Generic;
using Stein.Services;

namespace Stein.Models
{
    /// <summary>
    /// Represents a collection of SerializedItem objects that is ready to be injected into
    /// a template.
    /// </summary>
    public class Injectable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> Items { get; } = new();

        /// <summary>
        /// Initialize a new instance of the Injectable class with the collections from the
        /// given store.
        /// </summary>
        /// <param name="store">The store that collections will be pulled from.</param>
        /// <param name="config">
        /// A Configuration object used to control the method that is used to sort the collections
        /// </param>
        public Injectable(Store store, Configuration config)
        {
            SerializedItem configuration = new Configuration().GetConfigAllKeys();
            Dictionary<string, object> members = configuration.Pairs;

            foreach (var pair in members)
            {
                Items.Add(pair.Key, pair.Value);
            }

            foreach (Collection collection in store.Collections)
            {
                // Todo: Read configuration to find a sorting method.
                DateService.Sort(collection, DateService.SortMethod.LatestDate);

                List<SerializedItem> serializedMembers = new();

                foreach (Item item in collection.Items)
                {
                    serializedMembers.Add(item.Serialize());
                }

                Items.Add(collection.Info.Name, serializedMembers);
            }
        }
    }
}
