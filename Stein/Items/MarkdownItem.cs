using Markdig;
using Stein.Models;
using Stein.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stein.Collections
{
    /// <summary>
    /// Represents an Item derived from a Markdown file.
    /// </summary>
    public sealed class MarkdownItem : Item
    {
        /// <summary>
        /// Initialize a new instance of MarkdownItem with the given FileInfo and populate all
        /// available properties within the instance.
        /// </summary>
        public MarkdownItem(FileInfo fileInfo) : base(fileInfo)
        {
            Link = GetIterablePath(Info);
            Slug = StringService.Slugify(Path.GetFileNameWithoutExtension(Info.Name));

            string rawFile = PathService.ReadAllSafe(Info.FullName);

            if (String.IsNullOrEmpty(rawFile))
                return;

            YamlIndicators indicators = new(rawFile);

            if (indicators.NoYaml)
            {
                Invalidate(InvalidType.InvalidFrontmatter);
                Message.Log(Message.NoYaml(fileInfo));
            }

            Body = Markdown.ToHtml(rawFile[indicators.SecondEnd..].Trim());

            if (
                Issues.Contains(InvalidType.NoFrontmatter)
                || Issues.Contains(InvalidType.InvalidFrontmatter)
                )
                return;

            PopulateFrontmatter(indicators, rawFile);

            if (Template == null)
            {
                Message.Log(Message.NoTemplateKey(Info));
                return;
            }
        }

        /// <summary>
        /// Initialize a new instance of SerializedItem and make the properties of this instance of
        /// MarkdownItem available within the dynamic container of that new instance.
        /// </summary>
        /// <returns>
        /// Returns a new Instance of SerializedItem, containing a dynamic object where all 
        /// properties are available at the top level.
        /// </returns>
        public override SerializedItem Serialize()
        {
            dynamic serializedItem = new SerializedItem();
            SerializedItem castedItem = (SerializedItem)serializedItem;

            serializedItem.Link = Link;
            serializedItem.Date = Date;
            serializedItem.Body = Body;
            serializedItem.Slug = Slug;

            foreach (KeyValuePair<string, string> pair in Frontmatter)
            {
                castedItem.Add(pair.Key, pair.Value);
            }

            return serializedItem;
        }

        private Dictionary<string, string> Frontmatter { get; } = new();

        private string Body { get; set; }

        private void PopulateFrontmatter(YamlIndicators indicators, string rawFile)
        {
            Dictionary<string, string> rawPairs = new();

            try
            {
                string section = StringService.Slice(indicators.FirstEnd, indicators.SecondStart, rawFile);
                rawPairs = new YamlService().Deserialize(section);
            }
            catch (IndexOutOfRangeException)
            {
                Invalidate(InvalidType.InvalidFrontmatter);
                Message.Log(Message.InvalidKeyValuePair(Info));
            }

            if (Issues.Contains(InvalidType.InvalidFrontmatter))
                return;

            foreach (var (key, value) in rawPairs)
            {
                string point = key.ToLower();
                if (point == "date")
                    Date = value;
                else if (point == "template")
                    Template = value;
                else
                    Frontmatter.Add(key, value);
            }
        }
    }
}