using Stein.Collections;
using System.Collections.Generic;
using System.IO;
using Stein.Services;

namespace Stein.Models
{
    public class Collection
    {
        public Collection(string path) : this(new DirectoryInfo(path)) { }

        public Collection(DirectoryInfo directoryInfo)
        {
            Info = directoryInfo;

            FileInfo[] files = directoryInfo.GetFiles();
            foreach (var path in files)
            {
                if (PathService.IsIgnored(path.Name))
                    continue;

                if (path.Extension == "")
                {
                    Message.Log(Message.NoExtension(path));
                    continue;
                }

                Item item = path.Extension switch
                {
                    ".md" => new MarkdownItem(path),
                    _ => null
                };

                if (item is not MarkdownItem)
                {
                    Message message = new($"Format unsupported: {path.Name}", Message.InfoType.Error);
                    Message.Log(message);
                    continue;
                }

                if (item.IsInvalid) continue;

                Items.Add(item);
            }
        }

        public DirectoryInfo Info { get; }

        public List<Item> Items { get; } = new();
    }
}