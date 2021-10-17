using System.Collections.Generic;
using System.IO;
using Stein.Services;
using Stein.Items;

namespace Stein.Models
{
    /// <summary>
    /// Represents a group of Item objects.
    /// </summary>
    public class Collection
    {
        /// <summary>
        /// Initializes a new instance of the Collection class at the given path.
        /// </summary>
        /// <param name="path">A string representing the location of the collection.</param>
        public Collection(string path) : this(new DirectoryInfo(path)) { }

        /// <summary>
        /// Initializes a new instance of the Collection class with the given DirectoryInfo.
        /// </summary>
        /// <param name="directoryInfo">A DirectoryInfo derived from the directory.</param>
        public Collection(DirectoryInfo directoryInfo)
        {
            Info = directoryInfo;
            FileInfo[] files = directoryInfo.GetFiles();

            foreach (FileInfo path in files)
            {
                if (PathService.IsIgnored(path.Name))
                    continue;

                if (path.Extension == "")
                {
                    Message.Log(Message.NoExtension(path));
                    continue;
                }

                Item item = Item.GetItem(path);

                if (item is not MarkdownItem && item is not JsonItem)
                {
                    Message message = new Message($"Format unsupported: {path.Name}", Message.InfoType.Error);
                    Message.Log(message);
                    continue;
                }

                if (item.IsInvalid)
                    continue;

                Items.Add(item);
            }
        }

        /// <summary>
        /// A DirectoryInfo object derived from the directory.
        /// </summary>
        public DirectoryInfo Info { get; }

        /// <summary>
        /// The items associated with this collection.
        /// </summary>
        public List<Item> Items { get; } = new List<Item>();
    }
}