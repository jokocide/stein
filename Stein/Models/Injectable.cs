﻿using System.Collections.Generic;
using Stein.Services;

namespace Stein.Models
{
    public class Injectable
    {
        public Dictionary<string, object> Items { get; } = new();

        public static Injectable Assemble(Store store, Configuration config)
        {
            SerializedItem configuration = new Configuration().GetConfigAllKeys();
            Dictionary<string, object> members = configuration.Pairs;

            Injectable injectable = new();

            foreach (var pair in members)
            {
                injectable.Items.Add(pair.Key, pair.Value);
            }

            foreach (Collection collection in store.Collections)
            {
                // Todo: Read configuration to find a sorting method.
                DateService.Sort(collection, DateService.SortMethod.LatestDate);

                List<SerializedItem> serializedMembers = new();

                foreach(Item item in collection.Items)
                {
                    serializedMembers.Add(item.Serialize());
                }

                injectable.Items.Add(collection.Info.Name, serializedMembers);
            }

            return injectable;
        }
    }
}
