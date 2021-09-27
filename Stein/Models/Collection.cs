using Stein.Collections;
using Stein.Interfaces;
using Stein.Services;
using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    public class Collection : ISerializer
    {
        public Collection(DirectoryInfo directoryInfo) => Info = directoryInfo;

        public DirectoryInfo Info { get; }

        public List<Item> Items { get; } = new();

        public static IEnumerable<Collection> GetCollection(IEnumerable<string> directories)
        {
            List<Collection> collections = new();

            foreach (string directory in directories)
            {
                Collection collection = GetCollection(directory);
                collections.Add(collection);
            }

            return collections;
        }

        public static Collection GetCollection(string directory)
        {
            DirectoryInfo info = new(directory);

            Collection collection = new(info);

            FileInfo[] files = info.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension == "")
                {
                    MessageService.Log(Message.NoExtension(file));
                    continue;
                }

                Item item = Item.GetItem(file);

                if (item is not MarkdownItem)
                {
                    Message message = new($"Format unsupported: {item.Info.Name}", Message.InfoType.Error);
                    MessageService.Log(message);
                    continue;
                }

                collection.Items.Add(item);
            }

            return collection;
        }

        public SerializedItem Serialize()
        {
            SerializedItem serializedCollection = new();
            List<SerializedItem> serializedMembers = new();

            serializedCollection.Add(Info.Name, serializedMembers);

            Items.ForEach(item =>
            {
                ISerializer castedItem = item as ISerializer;

                SerializedItem serializedMember = castedItem.Serialize();
                serializedMembers.Add(serializedMember);
            });

            return serializedCollection;
        }
    }
}