using System;
using System.IO;
using HandlebarsDotNet;
using Stein.Items;
using Stein.Services;

namespace Stein.Models
{
    /// <summary>
    /// Represents an object that has been fully processed and is ready to be written
    /// out to the file system.
    /// </summary>
    public class Writable
    {
        /// <summary>
        /// Initialize a new instance of the Writable class with the provided file and payload.
        /// </summary>
        /// <param name="file">A FileInfo derived from the file.</param>
        /// <param name="payload">The text to be written out.</param>
        public Writable(FileInfo file, string payload)
        {
            Payload = payload;
            Target = GetOutputPath(file);
        }

        /// <summary>
        /// The location that Payload will be written out to.
        /// </summary>
        public string Target { get; }

        /// <summary>
        /// The text to be written out to a file at Target.
        /// </summary>
        public string Payload { get; }

        /// <summary>
        /// Commit writable to disk.
        /// </summary>
        /// <param name="writable">The Writable to be written out to the file system.</param>
        public static void Write(Writable writable)
        {
            string directory = Path.GetDirectoryName(writable.Target);
            Directory.CreateDirectory(directory);
            File.WriteAllText(writable.Target, writable.Payload);
        }

        /// <summary>
        /// Create a new Writable object from item.
        /// </summary>
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
            if (item.Template == null)
                return null;

            string rawTemplate;
            string commonPath = Path.Join(PathService.GetTemplatesPath(), item.Template);

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
            string relative = Path.GetRelativePath(PathService.GetResourcesPath(), file.FullName);
            // Configuration config = new Configuration().GetConfig();

            if (fileNameNoExtension == "index")
            {
                return Path.Join(PathService.GetSitePath(), "index.html");
            }
            else
            {
                string name = Path.GetFileNameWithoutExtension(relative);
                string parent = Path.GetDirectoryName(relative);
                string directory = Path.Join(PathService.GetSitePath(), parent, name);
                Directory.CreateDirectory(directory);
                return Path.Join(directory, "index.html");
            }
        }
    }
}