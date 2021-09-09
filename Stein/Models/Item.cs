using Stein.Collections;
using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    /// <summary>
    /// Base class for all Resource types. A Resource represents a file that has been discovered in a project,
    /// and provides properties and methods to store and manipulate the data in that file.
    /// </summary>
    public abstract class Item
    {
        /// <summary>
        /// Create an Item suitable for the extension of the received FileInfo object.
        /// </summary>
        /// <param name="extension">A FileInfo object derived from the file.</param>
        /// <returns>An Item object.</returns>
        public static Item GetItem(FileInfo fileInfo)
        {
            return fileInfo.Extension switch
            {
                ".md" => new MarkdownItem(fileInfo),
                ".csv" => new CsvItem(fileInfo),
                ".json" => new JsonItem(fileInfo),
                ".toml" => new TomlItem(fileInfo),
                ".xml" => new XmlItem(fileInfo),
                _ => null
            };
        }

        /// <summary>
        /// Classes deriving from Resource are responsible for providing a method to retrieve data from that
        /// type of resource.
        /// </summary>
        internal abstract void Process(Store store);

        /// <summary>
        /// Return all data from a resource in a format suitable for template injection.
        /// </summary>
        /// <returns>
        /// A representation of this object in a format that is ready to inject into a template.
        /// </returns>
        internal abstract Injectable Serialize();

        /// <summary>
        /// Stores the name of a requested template file.
        /// </summary>
        internal string Template { get; set; }

        /// <summary>
        /// A string representing the output path for a file, injected to allow for generating links
        /// during iteration in a template.
        /// </summary>
        internal string Link { get; set; }

        /// <summary>
        /// A string the represents the slugified version of this Resource's file name.
        /// </summary>
        internal string Slug { get; set; }

        /// <summary>
        /// User-provided string to represent a date, used during sorting if it can be parsed into a
        /// DateTime object.
        /// </summary>
        internal string Date { get; set; }

        /// <summary>
        /// Provides access to the file that this Resource represents.
        /// </summary>
        internal FileInfo Info { get; }

        /// <summary>
        /// Invalidate the Metadata with a given InvalidType.
        /// </summary>
        /// <param name="type">
        /// An InvalidType to describe the cause of invalidation.
        /// </param>
        internal void Invalidate(InvalidType type)
        {
            if (!IsInvalid) IsInvalid = true;
            Issues.Add(type);
        }

        /// <summary>
        /// If an error is encountered during Process() this will be true, indicating something is wrong with the
        /// resource. It will not be fully processed or made into a Writable as a consequence. 
        /// </summary>
        internal bool IsInvalid { get; private set; }

        /// <summary>
        /// Issues found during Process() are recorded here.
        /// </summary>
        internal List<InvalidType> Issues { get; } = new();

        protected Item(FileInfo fileInfo) => Info = fileInfo;

        internal enum InvalidType
        {
            InvalidFrontmatter,
            NoFrontmatter,
            TemplateNotFound,
            NoTemplate
        }
    }
}