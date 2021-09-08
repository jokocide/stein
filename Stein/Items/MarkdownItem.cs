using Markdig;
using Stein.Models;
using Stein.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stein.Collections
{
    /// <summary>
    /// Represents a Markdown file.
    /// </summary>
    public sealed class MarkdownItem : CollectionItem
    {
        /// <summary>
        /// Contains all YAML Frontmatter.
        /// </summary>
        internal Dictionary<string, string> Frontmatter { get; } = new();

        /// <summary>
        /// Stores the Markdown body, which is everything in the file except for the YAML frontmatter.
        /// </summary>
        private string Body { get; set; }

        public MarkdownItem(FileInfo fileInfo) : base(fileInfo)
        {
            Link = PathService.GetIterablePath(Info);
        }

        /// <summary>
        /// Return all data in a format suitable for template injection.
        /// </summary>
        /// <returns>
        /// A dynamic object that is ready to be injected into a template.
        /// </returns>
        internal override Injectable Serialize()
        {
            dynamic injectable = new Injectable();
            Injectable castedInjectable = (Injectable)injectable;

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

        /// <summary>
        /// Populate the properties of this Resource.
        /// </summary>
        internal override void Process(Store store)
        {
            // Raw contents of the file will be stored here.
            string rawFile = null;

            // Reading with ReadWrite on FileAccess & FileShare will prevent IOException during ServeRoutine.
            using (var stream = File.Open(Info.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var reader = new StreamReader(stream);
                rawFile = reader.ReadToEnd();
            }

            // Ignore empty files.
            if (String.IsNullOrEmpty(rawFile)) return;

            // Develop a 'slug' based on the file name.
            Slug = StringService.Slugify(Path.GetFileNameWithoutExtension(Info.Name));

            // The location of the frontmatter key/value pairs within rawFile are stored here.
            (int FirstStart, int FirstEnd, int SecondStart, int SecondEnd) indices = (0, 0, 0, 0);

            // Determine the distance between the first '---' and second '---'.
            int openBlock = rawFile.IndexOf("---");
            int closeBlock = rawFile.IndexOf("---", 2);

            // If the file does not have two instances of '---' or no content is between them.
            if (openBlock == -1 || closeBlock == -1 || closeBlock - openBlock <= 5)
            {
                Invalidate(InvalidType.InvalidFrontmatter);
                MessageService.Log(new Message($"No YAML: {Info.Name}", Message.InfoType.Error));
            }
            else // Attempt to populate indices.
            {
                indices = YamlService.GetIndices(rawFile);
            }

            // If a problem occurred while getting indices, it will return as (0, 0, 0, 0). We can mark the
            // file as invalid, but we only do so if the file is not already invalid.
            if (indices == (0, 0, 0, 0) && !Issues.Contains(InvalidType.InvalidFrontmatter))
            {
                Invalidate(InvalidType.InvalidFrontmatter);
                MessageService.Log(new Message($"Invalid YAML: {Info.Name}", Message.InfoType.Error));
            }

            string untransformedBody = rawFile[indices.SecondEnd..].Trim();
            string transformedBody = Markdown.ToHtml(untransformedBody);

            Body = transformedBody;

            if (Issues.Contains(InvalidType.NoFrontmatter) ||
                Issues.Contains(InvalidType.InvalidFrontmatter)) return;

            Dictionary<string, string> rawPairs = new();

            try
            {
                string yamlSection = StringService.Slice(indices.FirstEnd, indices.SecondStart, rawFile).Trim();
                rawPairs = YamlService.Deserialize(yamlSection);
            }
            catch (IndexOutOfRangeException)
            {
                Invalidate(InvalidType.InvalidFrontmatter);
                MessageService.Log(new Message($"Invalid key/value pair in YAML: {Info.Name}", Message.InfoType.Error));
            }

            if (Issues.Contains(InvalidType.InvalidFrontmatter)) return;

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

            Writable writable;

            try
            {
                writable = Writable.GetWritable(this);
            }
            catch (FileNotFoundException)
            {
                Invalidate(InvalidType.TemplateNotFound);
                MessageService.Log(Message.TemplateNotFound(Info));
                return;
            }

            store.Writable.Add(writable);
        }
    }
}