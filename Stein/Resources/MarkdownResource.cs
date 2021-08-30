using Markdig;
using Stein.Models;
using Stein.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Stein.Metadata
{
    /// <summary>
    /// Represents a Markdown file.
    /// </summary>
    public sealed class MarkdownResource : Resource
    {
        /// <summary>
        /// Contains all YAML Frontmatter.
        /// </summary>
        internal Dictionary<string, string> Frontmatter { get; } = new();

        /// <summary>
        /// Stores the Markdown body, which is everything in the file except for the YAML frontmatter.
        /// </summary>
        private string Body { get; set; }

        public MarkdownResource(FileInfo fileInfo) : base(fileInfo)
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
        internal override void Process()
        {
            string rawFile = null;

            try
            {
                rawFile = File.ReadAllText(Info.FullName);
            }
            catch (IOException)
            {
                Thread.Sleep(10);
                rawFile = File.ReadAllText(Info.FullName);
            }

            // Skip empty files.
            if (rawFile.Length <= 0) return;

            Slug = StringService.Slugify(Path.GetFileNameWithoutExtension(Info.Name));

            (int FirstStart, int FirstEnd, int SecondStart, int SecondEnd) indices = new(0, 0, 0, 0);

            if (rawFile.Substring(0, 3) == "---")
            {
                // First three characters being "---" in a Markdown file indicates YAML frontmatter,
                // so we call GetIndices to determine where it actually is in the file and make sure
                // that it appears to be in a valid format. (No missing ':' between key/value pairs, etc)
                try
                {
                    indices = YamlService.GetIndices(rawFile);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Invalidate(InvalidType.InvalidFormat);

                    string path = Path.Join("collections", Info.DirectoryName, Info.Name);
                    string error = $"({Info.Name}) Found invalid YAML.";
                    MessageService.Log(new Message(error, Message.InfoType.Error));
                }
            }
            else
            {
                Invalidate(InvalidType.NoFrontmatter);
            }

            // This should accurately retrieve the body of the Markdown file whether it
            // has YAML frontmatter or not.
            string untransformedBody = rawFile[indices.SecondEnd..].Trim();
            string transformedBody = Markdown.ToHtml(untransformedBody);
            Body = transformedBody;

            if (Issues.Contains(InvalidType.NoFrontmatter) || Issues.Contains(InvalidType.InvalidFormat))
                return;

            Dictionary<string, string> rawPairs = new();

            try
            {
                rawPairs = YamlService.Deserialize(
                    StringService.Slice(indices.FirstEnd, indices.SecondStart, rawFile).Trim()
                );
            }
            catch (IndexOutOfRangeException)
            {
                Invalidate(InvalidType.InvalidFormat);

                string error = $"({Info.Name}) YAML contains invalid key/value pair.";
                MessageService.Log(new Message(error, Message.InfoType.Error));
            }

            if (Issues.Contains(InvalidType.InvalidFormat)) return;

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
        }
    }
}