using Markdig;
using Stein.Interfaces;
using Stein.Models;
using Stein.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stein.Collections
{
    public sealed class MarkdownItem : Item, ISerializer
    {
        public MarkdownItem(FileInfo fileInfo) : base(fileInfo)
        {
            Link = PathService.GetIterablePath(Info);
            Slug = StringService.Slugify(Path.GetFileNameWithoutExtension(Info.Name));

            string rawFile = PathService.ReadAllSafe(Info.FullName);

            if (String.IsNullOrEmpty(rawFile))
                return;

            YamlIndicators indicators = new(rawFile);

            if (indicators.NoYaml)
            {
                int bonk = indicators.SecondEnd;
                Invalidate(InvalidType.InvalidFrontmatter);
                MessageService.Log(new Message($"No YAML: {Info.Name}", Message.InfoType.Warning));
            }

            Body = Markdown.ToHtml(rawFile[indicators.SecondEnd..].Trim());

            if (Issues.Contains(InvalidType.NoFrontmatter) || Issues.Contains(InvalidType.InvalidFrontmatter))
                return;

            Dictionary<string, string> rawPairs = new();

            try
            {
                string section = StringService.Slice(indicators.FirstEnd, indicators.SecondStart, rawFile);
                rawPairs = new YamlService().Deserialize(section);
            }
            catch (IndexOutOfRangeException)
            {
                Invalidate(InvalidType.InvalidFrontmatter);
                MessageService.Log(new Message($"Invalid key/value pair in YAML: {Info.Name}", Message.InfoType.Error));
            }

            if (Issues.Contains(InvalidType.InvalidFrontmatter))
                return;

            foreach (var (key, value) in rawPairs)
            {
                switch (key.ToLower())
                {
                    case "date":
                        Date = value;
                        break;
                    case "template":
                        Template = value;
                        break;
                    default:
                        Frontmatter.Add(key, value);
                        break;
                }
            }

            if (Template == null)
            {
                MessageService.Log(Message.NoTemplateKey(Info));
                return;
            }

            //Writable writable;

            //try
            //{
            //    writable = Writable.GetWritable(this);
            //}
            //catch (FileNotFoundException)
            //{
            //    Invalidate(InvalidType.TemplateNotFound);
            //    MessageService.Log(Message.TemplateNotFound(Info));
            //    return;
            //}

            //return writable;
        }

        public SerializedItem Serialize()
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
    }
}