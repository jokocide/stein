using Stein.Collections;
using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    public class Collection
    {
        public Collection(DirectoryInfo directoryInfo) => Info = directoryInfo;

        public DirectoryInfo Info { get; }

        public List<Item> Items { get; } = new();

        public static Collection GetCollection(string directory)
        {
            DirectoryInfo info = new(directory);

            Collection collection = new(info);

            FileInfo[] files = info.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Name.StartsWith("_")) continue;

                if (file.Extension == "")
                {
                    Message.Log(Message.NoExtension(file));
                    continue;
                }

                Item item = Item.GetItem(file);

                if (item is not MarkdownItem)
                {
                    Message message = new($"Format unsupported: {item.Info.Name}", Message.InfoType.Error);
                    Message.Log(message);
                    continue;
                }

                collection.Items.Add(item);
            }

            return collection;
        }
    }
}