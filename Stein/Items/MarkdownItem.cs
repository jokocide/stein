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
        public MarkdownItem(FileInfo fileInfo) : base(fileInfo) => Link = PathService.GetIterablePath(Info);

        public MarkdownItem(FileInfo fileInfo) : base(fileInfo)
        {
            Link = PathService.GetIterablePath(Info);
            Slug = StringService.Slugify(Path.GetFileNameWithoutExtension(Info.Name));
        }

        public Dictionary<string, string> Frontmatter { get; } = new();

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

        public override Writable Process()
        {
            string rawFile = PathService.ReadAllSafe(Info.FullName);

            if (String.IsNullOrEmpty(rawFile)) return null;

            // (int FirstStart, int FirstEnd, int SecondStart, int SecondEnd) indices = (0, 0, 0, 0);
            // int openBlock = rawFile.IndexOf("---");
            // int closeBlock = rawFile.IndexOf("---", 2);
            YamlIndicators indicators = new(rawFile);

            if (indicators.NoYaml)
            {
                Invalidate(InvalidType.InvalidFrontmatter);
                MessageService.Log(new Message($"No YAML: {Info.Name}", Message.InfoType.Warning));
            }
            else
            {
                indices = new YamlService().GetIndicatorIndices(rawFile);
            }

            if (indices == (0, 0, 0, 0) && !Issues.Contains(InvalidType.InvalidFrontmatter))
            {
                Invalidate(InvalidType.InvalidFrontmatter);
                MessageService.Log(new Message($"Invalid YAML: {Info.Name}", Message.InfoType.Error));
            }

            string untransformedBody = rawFile[indices.SecondEnd..].Trim();
            Body = Markdown.ToHtml(untransformedBody);

            if (Issues.Contains(InvalidType.NoFrontmatter) ||
                Issues.Contains(InvalidType.InvalidFrontmatter)) return null;

            Dictionary<string, string> rawPairs = new();

            try
            {
                string yamlSection = StringService.Slice(indices.FirstEnd, indices.SecondStart, rawFile).Trim();
                rawPairs = new YamlService().Deserialize(yamlSection);
            }
            catch (IndexOutOfRangeException)
            {
                Invalidate(InvalidType.InvalidFrontmatter);
                MessageService.Log(new Message($"Invalid key/value pair in YAML: {Info.Name}", Message.InfoType.Error));
            }

            if (Issues.Contains(InvalidType.InvalidFrontmatter)) return null;

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
                return null;
            }

            Writable writable;

            try
            {
                writable = Writable.GetWritable(this);
            }
            catch (FileNotFoundException)
            {
                Invalidate(InvalidType.TemplateNotFound);
                MessageService.Log(Message.TemplateNotFound(Info));
                return null;
            }

            return writable;
        }

        private string Body { get; set; }
    }
}