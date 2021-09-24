using System.Collections.Generic;
using Stein.Interfaces;
using Stein.Services;

namespace Stein.Models
{
    class Injectable
    {
        public Dictionary<string, object> Items { get; } = new();

        public static Injectable Assemble(Store store, Configuration config)
        {
            SerializedItem configuration = new ConfigurationService().Serialize();
            Dictionary<string, object> members = configuration.Pairs;

            Injectable injectable = new();

            foreach (var pair in members)
                injectable.Items.Add(pair.Key, pair.Value);

            foreach (Collection collection in store.Collections)
            {
                // Todo: Read configuration to find a sorting method.
                DateService.Sort(collection, DateService.SortMethod.LatestDate);

                List<SerializedItem> serializedMembers = new();
                //collection.Items.ForEach(item => serializedMembers.Add(item.Serialize()));
                collection.Items.ForEach(item =>
                {
                    ISerializer castedItem = item as ISerializer;
                    serializedMembers.Add(castedItem.Serialize());
                });

                injectable.Items.Add(collection.Info.Name, serializedMembers);
            }

            return injectable;
        }
    }
}
