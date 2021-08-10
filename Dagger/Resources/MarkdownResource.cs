using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Dagger.Models;
using Dagger.Services;
using HandlebarsDotNet;
using Markdig;

namespace Dagger.Metadata
{
    /// <summary>
    /// Represents a Markdown file.
    /// </summary>
    public sealed class MarkdownResource : Resource
    {
        // internal override KeyValueStore Store { get; } = new();

        /// <summary>
        /// Stores YAML frontmatter.
        /// </summary>
        internal Dictionary<string, string> Frontmatter { get; } = new();
        
        /// <summary>
        /// Stores the Markdown body, which is everything in the file except for the YAML frontmatter.
        /// </summary>
        internal string Body { get; set; }

        public MarkdownResource(FileInfo fileInfo) : base(fileInfo)
        {
            Link = PathService.GetOutputPath(Info);
        }
        
        /// <summary>
        /// Return all data in a format suitable for template injection.
        /// </summary>
        /// <returns></returns>
        internal override Injectable Serialize()
        {
            dynamic injectable = new Injectable();
            Injectable castedInjectable = (Injectable)injectable;
            
            injectable.Link = Link;
            injectable.Date = Date;
            injectable.Body = Body;

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
            string rawFile;
            try
            {
                rawFile = File.ReadAllText(Info.FullName);
            }
            catch (IOException)
            {
                Thread.Sleep(200);
                rawFile = File.ReadAllText(Info.FullName);
            }
                    
            (int FirstStart, int FirstEnd, int SecondStart, int SecondEnd) indices = new();
            try
            {
                indices = YamlService.GetIndices(rawFile);
            }
            catch (ArgumentOutOfRangeException)
            {
                Invalidate(InvalidType.InvalidFormat);
            }

            if (IsInvalid) return;

            Dictionary<string, string> rawPairs = YamlService.Deserialize(
                StringService.Slice(indices.FirstEnd, indices.SecondStart, rawFile).Trim()
            );

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
                }
            }
            
            string untransformedBody = rawFile[indices.SecondEnd..].Trim();
            string transformedBody = Markdown.ToHtml(untransformedBody);
            Body = transformedBody;
            
            if (Template == null) return;

            string rawTemplate = null;
            try
            {
                rawTemplate = File.ReadAllText(Path.Join(PathService.TemplatesPath, Template + ".hbs"));
            }
            catch (FileNotFoundException)
            {
                Invalidate(InvalidType.TemplateNotFound);
            }
                
            if (IsInvalid) return;
                
            HandlebarsTemplate<object, object> compiledTemplate = Handlebars.Compile(rawTemplate);
            string renderedTemplate = compiledTemplate(Body);

            Writable writable = new(Info, renderedTemplate);
            StoreService.Writable.Add(writable);
        }
    }
}