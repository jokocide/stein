using System;
using System.IO;
using HandlebarsDotNet;
using Stein.Collections;
using Stein.Services;

namespace Stein.Models
{
    /// <summary>
    /// Represents a fully processed item that is ready to be written to the file system.
    /// </summary>
    public class Writable
    {
        /// <summary>
        /// A string path that represents the desired location of Payload.
        /// </summary>
        public string Target { get; }

        /// <summary>
        ///  The data to be written to the path in Target.
        /// </summary>
        public string Payload { get; }

        /// <summary>
        /// Create a new Writable object.
        /// </summary>
        /// <param name="file"> A FileInfo object used to generate the Writable.</param>
        /// <param name="payload">The string content to be written.</param>
        public Writable(FileInfo file, string payload)
        {
            Payload = payload;
            Target = PathService.GetOutputPath(file);
        }

        /// <summary>
        /// A factory method to create a new Writable from a MarkdownResource.
        /// </summary>
        /// <param name="resource">A MarkdownResource object to be converted.</param>
        /// <returns> A new Writable based on the given MarkdownResource.</returns>
        public static Writable GetWritable(MarkdownItem resource)
        {
            // Attempt to read the template file, this will throw a FileNotFoundException
            // if the template is not found in the expected location.
            string rawTemplate;
            string commonPath = Path.Join(PathService.TemplatesPath, resource.Template);
            FileStream stream;

            // Template has an extension.
            if (Path.HasExtension(resource.Template))
            {
                stream = File.Open(commonPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }

            // Template has no extension.
            else
            {
                stream = File.Open(commonPath + ".hbs", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }

            StreamReader reader = new StreamReader(stream);
            rawTemplate = reader.ReadToEnd();

            stream.Close();

            // Retrieve an injectable.
            Injectable injectable = resource.Serialize();

            // Compile, render and return.
            var compiledTemplate = Handlebars.Compile(rawTemplate);
            string renderedTemplate = compiledTemplate(injectable);
            return new Writable(resource.Info, renderedTemplate);
        }

        /// <summary>
        /// A factory method to create a new Writable from a JsonResource.
        /// </summary>
        /// <param name="resource">A JsonResource object to be converted.</param>
        /// <returns>A new Writable based on the given JsonResource.</returns>
        public static Writable GetWritable(JsonItem resource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A factory method to create a new Writable from a CsvResource.
        /// </summary>
        /// <param name="resource">A CsvResource object to be converted.</param>
        /// <returns>A new Writable based on the given CsvResource.</returns>
        public static Writable GetWritable(CsvItem resource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A factory method to create a new Writable from a TomlResource.
        /// </summary>
        /// <param name="resource">A TomlResource object to be converted.</param>
        /// <returns>A new Writable based on the given TomlResource.</returns>
        public static Writable GetWritable(TomlItem resource)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A factory method to create a new Writable from an XmlResource.
        /// </summary>
        /// <param name="resource">A XmlResource object to be converted.</param>
        /// <returns>A new Writable based on the given XmlResource.</returns>
        public static Writable GetWritable(XmlItem resource)
        {
            throw new NotImplementedException();
        }
    }
}