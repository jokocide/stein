using System;
using System.IO;
using HandlebarsDotNet;
using Stein.Collections;
using Stein.Services;

namespace Stein.Models
{
    public class Writable
    {
        public Writable(FileInfo file, string payload)
        {
            Payload = payload;
            Target = PathService.GetOutputPath(file);
        }

        public string Target { get; }

        public string Payload { get; }

        public static Writable GetWritable(Item item)
        {
            switch (item)
            {
                case MarkdownItem markdownItem:
                    return GetWritable(markdownItem);
                case JsonItem jsonItem:
                    return GetWritable(jsonItem);
                case CsvItem csvItem:
                    return GetWritable(csvItem);
                case TomlItem tomlItem:
                    return GetWritable(tomlItem);
                case XmlItem xmlItem:
                    return GetWritable(xmlItem);
                default:
                    return null;
            }
        }

        public static Writable GetWritable(JsonItem resource) => null;

        public static Writable GetWritable(CsvItem resource) => null;

        public static Writable GetWritable(TomlItem resource) => null;

        public static Writable GetWritable(XmlItem resource) => null;

        public static Writable GetWritable(MarkdownItem item)
        {
            string rawTemplate;
            string commonPath = Path.Join(PathService.TemplatesPath, item.Template);

            string resolvedPath;
            FileStream stream;

            if (Path.HasExtension(item.Template))
            {
                resolvedPath = commonPath;
            }
            else
            {
                resolvedPath = commonPath + ".hbs";
            }

            stream = File.Open(resolvedPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            StreamReader reader = new StreamReader(stream);
            rawTemplate = reader.ReadToEnd();

            stream.Close();

            SerializedItem injectable = item.Serialize();

            var compiledTemplate = Handlebars.Compile(rawTemplate);
            string renderedTemplate = compiledTemplate(injectable);
            return new Writable(item.Info, renderedTemplate);
        }
    }
}