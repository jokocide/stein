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
    public class MarkdownMetadata : Metadata
    {
        private string Body { get; set; }
        
        public Dictionary<string, string> Frontmatter { get; }

        public MarkdownMetadata(FileInfo fileInfo) : base(fileInfo)
        {
            Frontmatter = new Dictionary<string, string> { { "path", ResourcePath } };
        }

        public override void Process()
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
                Invalidate(InvalidType.Format);
            }

            if (IsInvalid) return;

            Dictionary<string, string> rawMetadata = YamlService.CreateMetadata(
                StringService.Slice(indices.FirstEnd, indices.SecondStart, rawFile).Trim()
            );

            foreach (var (key, value) in rawMetadata)
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
                
                Frontmatter.Add(key, value);
            }
            
            if (Template == null) return;
            
            string untransformedBody = rawFile[indices.SecondEnd..].Trim();
            string transformedBody = Markdown.ToHtml(untransformedBody);

            Body = transformedBody;
            Frontmatter.Add("body", transformedBody);

            string rawTemplate = null;
                
            try
            {
                rawTemplate = File.ReadAllText(Path.Join(PathService.TemplatesPath, Template + ".hbs"));
            }
            catch (FileNotFoundException)
            {
                Invalidate(InvalidType.MissingTemplate);
            }
                
            if (IsInvalid) return;
                
            HandlebarsTemplate<object, object> compiledTemplate = Handlebars.Compile(rawTemplate);
                
            string renderedTemplate = compiledTemplate(Frontmatter);

            Writable writable = new(Info, renderedTemplate);
            StoreService.Writable.Add(writable);
        }
    }
}