using Stein.Collections;
using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    public abstract class Item
    {
        public string Template { get; set; }

        public string Link { get; set; }

        public string Slug { get; set; }

        public string Date { get; set; }

        public bool IsInvalid { get; private set; }

        public FileInfo Info { get; }

        public static Item GetItem(FileInfo fileInfo)
        {
            Item item;
            string extension = fileInfo.Extension;

            switch (extension)
            {
                case ".md":
                    item = new MarkdownItem(fileInfo);
                    break;
                case ".csv":
                    item = new CsvItem(fileInfo);
                    break;
                case ".json":
                    item = new JsonItem(fileInfo);
                    break;
                case ".toml":
                    item = new TomlItem(fileInfo);
                    break;
                case ".xml":
                    item = new XmlItem(fileInfo);
                    break;
                default:
                    item = null;
                    break;
            }

            return item;
        }

        public List<InvalidType> Issues { get; } = new();

        public void Invalidate(InvalidType type)
        {
            if (!IsInvalid) IsInvalid = true;
            Issues.Add(type);
        }

        public enum InvalidType
        {
            InvalidFrontmatter,
            NoFrontmatter,
            TemplateNotFound,
            NoTemplate
        }

        protected Item(FileInfo fileInfo) => Info = fileInfo;
    }
}