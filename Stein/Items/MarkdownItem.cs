using Markdig;
using Stein.Models;
using Stein.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stein.Collections
{
    public sealed class MarkdownItem : Item
    {
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

        public override SerializedItem Serialize()
        {
            dynamic injectable = new SerializedItem();
            SerializedItem castedInjectable = (SerializedItem)injectable;

            injectable.Link = Link;
            injectable.Date = Date;
            injectable.Body = Body;
            injectable.Slug = Slug;

            foreach (KeyValuePair<string, string> pair in Frontmatter)
            {
                castedInjectable.Add(pair.Key, pair.Value);
            }

            return injectable;
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