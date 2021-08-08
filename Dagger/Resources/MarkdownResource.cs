using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Dagger.Models;
using Dagger.Services;
using Dagger.Storage;
using HandlebarsDotNet;
using Markdig;

namespace Dagger.Metadata
{
    /// <summary>
    /// Represents a Markdown file.
    /// </summary>
    public sealed class MarkdownResource : Resource
    {
        /// <summary>
        /// Data stores the YAML frontmatter found in a Markdown file. Certain keys like 'date' and 'template'
        /// are stored in top-level properties on Store.
        /// </summary>
        internal override KeyValueStore Store { get; } = new();

        public MarkdownResource(FileInfo fileInfo) : base(fileInfo)
        {
            Store.Link = PathService.GetOutputPath(Info);
        }
        
        /// <summary>
        /// Attempt to parse the data in this resource into a MarkdownResource object.
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
                        Store.Date = value;
                        break;
                    case "template":
                        Store.Template = value;
                        break;
                }
            }
            
            string untransformedBody = rawFile[indices.SecondEnd..].Trim();
            string transformedBody = Markdown.ToHtml(untransformedBody);
            Store.Body = transformedBody;
            
            if (Store.Template == null) return;

            string rawTemplate = null;
            try
            {
                rawTemplate = File.ReadAllText(Path.Join(PathService.TemplatesPath, Store.Template + ".hbs"));
            }
            catch (FileNotFoundException)
            {
                Invalidate(InvalidType.TemplateNotFound);
            }
                
            if (IsInvalid) return;
                
            HandlebarsTemplate<object, object> compiledTemplate = Handlebars.Compile(rawTemplate);
            string renderedTemplate = compiledTemplate(Store.Body);

            Writable writable = new(Info, renderedTemplate);
            MemoryService.Writable.Add(writable);
        }
    }
}