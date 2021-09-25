using Stein.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    public class Collection : ISerializer
    {
        public Collection(DirectoryInfo directoryInfo) => Info = directoryInfo;

        public DirectoryInfo Info { get; }

        public List<Item> Items { get; } = new();

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

        public static IEnumerable<Collection> GetCollection(IEnumerable<string> directories)
        {
            //foreach (string path in PathService.CollectionsDirectories)
            //{
            //    DirectoryInfo info = new(path);
            //    Collection collection = new(info);

            //    foreach (FileInfo file in info.GetFiles())
            //    {
            //        if (file.Extension == "")
            //        {
            //            MessageService.Log(Message.NoExtension(file));
            //            continue;
            //        }

            //        Item item = Item.GetItem(file);

            //        if (item is not MarkdownItem)
            //        {
            //            MessageService.Log(new Message($"Format unsupported: {item.Info.Name}", Message.InfoType.Error));
            //            continue;
            //        }

            //        collection.Items.Add(item);
            //        Writable writable = Writable.GetWritable(item);

            //        if (writable == null)
            //            continue;

            //        Store.Writable.Add(writable);
            //    }

            //    Store.Collections.Add(collection);
            //}
        }

        public static Collection GetCollection(string directory) { }
    }
}