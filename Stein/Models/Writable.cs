using System.Collections.Generic;
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

        public static void Write(IEnumerable<Writable> writables)
        {
            foreach (Writable writable in writables) Write(writable);
        }

        public static void Write(Writable writable)
        {
            string directory = Path.GetDirectoryName(writable.Target);
            Directory.CreateDirectory(directory);
            File.WriteAllText(writable.Target, writable.Payload);
        }

        public static IEnumerable<Writable> GetWritable(IEnumerable<Collection> collections)
        {
            List<Writable> writables = new();

            foreach(Collection collection in collections)
            {
                IEnumerable<Writable> newWritables = GetWritable(collection);
                writables.AddRange(newWritables);
            }

            return writables;

        }

        public static IEnumerable<Writable> GetWritable(Collection collection)
        {
            List<Writable> writables = new();

            foreach(Item item in collection.Items)
                writables.Add(GetWritable(item));

            return writables;
        }

        public static IEnumerable<Writable> GetWritable(IEnumerable<Item> items)
        {
            List<Writable> writables = new();

            foreach(Item item in items)
                writables.Add(GetWritable(item));

            return writables;
        }

        public static Writable GetWritable(Item item)
        {
            Writable writable;

            switch (item)
            {
                case MarkdownItem markdownItem:
                    writable = GetWritable(markdownItem);
                    break;
                case JsonItem jsonItem:
                    writable = GetWritable(jsonItem);
                    break;
                case CsvItem csvItem:
                    writable =  GetWritable(csvItem);
                    break;
                case TomlItem tomlItem:
                    writable =  GetWritable(tomlItem);
                    break;
                case XmlItem xmlItem:
                    writable =  GetWritable(xmlItem);
                    break;
                default:
                    writable = null;
                    break;
            }

            return writable;
        }

        private static Writable GetWritable(JsonItem item) => null;

        private static Writable GetWritable(CsvItem item) => null;

        private static Writable GetWritable(TomlItem item) => null;

        private static Writable GetWritable(XmlItem item) => null;

        private static Writable GetWritable(MarkdownItem item)
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