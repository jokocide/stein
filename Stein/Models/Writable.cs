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

        public static Writable GetWritable(JsonItem resource) => throw new NotImplementedException();

        public static Writable GetWritable(CsvItem resource) => throw new NotImplementedException();

        public static Writable GetWritable(TomlItem resource) => throw new NotImplementedException();

        public static Writable GetWritable(XmlItem resource) => throw new NotImplementedException();

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

        public string Target { get; }

        public string Payload { get; }
    }
}