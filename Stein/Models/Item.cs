using Stein.Collections;
using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    public abstract class Item
    {
        public FileInfo Info { get; }

        public string Template { get; set; }

        public string Link { get; set; }

        public string Slug { get; set; }

        public string Date { get; set; }

        public bool IsInvalid { get; private set; }

        public List<InvalidType> Issues { get; } = new();

        public void Invalidate(InvalidType type)
        {
            if (!IsInvalid) IsInvalid = true;
            Issues.Add(type);
        }

        public static Item GetItem(FileInfo fileInfo)
        {
            return fileInfo.Extension switch
            {
                ".md" => new MarkdownItem(fileInfo),
                ".csv" => new CsvItem(fileInfo),
                ".json" => new JsonItem(fileInfo),
                ".toml" => new TomlItem(fileInfo),
                ".xml" => new XmlItem(fileInfo),
                _ => null
            };
        }

        public enum InvalidType
        {
            InvalidFrontmatter,
            NoFrontmatter,
            TemplateNotFound,
            NoTemplate
        }

        //public abstract Writable Process();

        protected Item(FileInfo fileInfo) => Info = fileInfo;
    }
}