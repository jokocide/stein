using Stein.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    public class Collection : ISerializable
    {
        public DirectoryInfo Info { get; }

        public List<Item> Items { get; } = new();

        public SerializedItem Serialize()
        {
            SerializedItem serializedCollection = new();
            List<SerializedItem> serializedMembers = new();

            serializedCollection.Add(Info.Name, serializedMembers);

            Items.ForEach(item =>
            {
                ISerializable castedItem = item as ISerializable;

                SerializedItem serializedMember = castedItem.Serialize();
                serializedMembers.Add(serializedMember);
            });

            return serializedCollection;
        }

        public Collection(DirectoryInfo directoryInfo) => Info = directoryInfo;
    }
}