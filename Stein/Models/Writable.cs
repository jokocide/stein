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
            Target = GetOutputPath(file);
        }

        public string Target { get; }

        public string Payload { get; }

        public static void Write(Writable writable)
        {
            string directory = Path.GetDirectoryName(writable.Target);
            Directory.CreateDirectory(directory);
            File.WriteAllText(writable.Target, writable.Payload);
        }

        public static Writable GetWritable(Item item)
        {
            Writable writable;

            switch (item)
            {
                case MarkdownItem markdownItem:
                    writable = GetWritable(markdownItem);
                    break;
                default:
                    writable = null;
                    break;
            }

            return writable;
        }

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

        private static string GetOutputPath(FileInfo file)
        {
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(file.Name);
            string relative = Path.GetRelativePath(PathService.ResourcesPath, file.FullName);
            Configuration config = new Configuration().GetConfig();

            if (relative == $"index.{config.Engine}")
            {
                return Path.Join(PathService.SitePath, "index.html");
            }
            else
            {
                string name = Path.GetFileNameWithoutExtension(relative);
                string parent = Path.GetDirectoryName(relative);
                string directory = Path.Join(PathService.SitePath, parent, name);
                Directory.CreateDirectory(directory);
                return Path.Join(directory, "index.html");
            }
        }
    }
}