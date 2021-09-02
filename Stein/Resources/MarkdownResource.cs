﻿using Markdig;
using Stein.Models;
using Stein.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Stein.Resources
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
        internal override void Process(Store store)
        {
            string rawFile = null;

            try
            {
                rawFile = File.ReadAllText(Info.FullName);
            }
            catch (IOException)
            {
                Thread.Sleep(100);
                rawFile = File.ReadAllText(Info.FullName);
            }

            // Skip empty files.
            if (String.IsNullOrEmpty(rawFile)) return;

            Slug = StringService.Slugify(Path.GetFileNameWithoutExtension(Info.Name));

            (int FirstStart, int FirstEnd, int SecondStart, int SecondEnd) indices = new(0, 0, 0, 0);

            if (rawFile.Substring(0, 3) == "---")
            {
                try
                {
                    indices = YamlService.GetIndices(rawFile);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Invalidate(InvalidType.InvalidFrontmatter);
                    MessageService.Log(new Message($"Invalid YAML: {Info.Name}", Message.InfoType.Error));
                    // return;
                }
            }
            else
            {
                Invalidate(InvalidType.NoFrontmatter);
            }

            // This should accurately retrieve the body of the Markdown file whether it
            // has YAML frontmatter or not, because indices.SecondEnd == 0.
            string untransformedBody = rawFile[indices.SecondEnd..].Trim();
            string transformedBody = Markdown.ToHtml(untransformedBody);

            Body = transformedBody;

            if (Issues.Contains(InvalidType.NoFrontmatter) || Issues.Contains(InvalidType.InvalidFrontmatter))
                return;

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

            if (Template == null) return;

            Writable writable;

            try
            {
                writable = Writable.CreateWritable(this);
            }
            catch (FileNotFoundException)
            {
                Invalidate(InvalidType.TemplateNotFound);
                MessageService.Log(new Message($"Unable to locate template: {Info.FullName}", Message.InfoType.Error));
                return;
            }
            catch (IOException)
            {
                Thread.Sleep(100);
                writable = Writable.CreateWritable(this);
            }

            store.Writable.Add(writable);
        }
    }
}