using Stein.Items;
using Stein.Services;
using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    /// <summary>
    /// Abstract base class for members shared by all inheriting classes.
    /// </summary>
    public abstract class Item
    {
        /// <summary>
        /// Contains the name of the requested template.
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Contains a string that can be used in templates to link to this file.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Contains a slug representation of the file name.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Contains the date derived from the frontmatter.
        /// </summary>
        public string Date 
        { 
            get { return _date; }
            set
            {
                _date = DateService.Format(value);
            }
        }

        /// <summary>
        /// Indicates the status of the object.
        /// </summary>
        /// <returns>
        /// Returns true if issues are encountered during the processing of this object.
        /// </returns>
        public bool IsInvalid { get; private set; }

        /// <summary>
        /// Contains a FileInfo object derived from the file.
        /// </summary>
        public FileInfo Info { get; }

        /// <summary>
        /// Contains the issues that were discovered during processing.
        /// </summary>
        public List<InvalidType> Issues { get; } = new List<InvalidType>();

        private string _date;

        public static Item GetItem(string path)
        {
            FileInfo info = new FileInfo(path);
            return GetItem(info);
        }

        /// <summary>
        /// Create a new Item based on path.
        /// </summary>
        public static Item GetItem(FileInfo path)
        {
            Item item = null;

            if (path.Extension == ".md")
                item = new MarkdownItem(path);
            if (path.Extension == ".json")
                item = new JsonItem(path);

            return item;
        }

        /// <summary>
        /// A facade method to facilitate the serialization of the item.
        /// </summary>
        public abstract SerializedItem Serialize();

        /// <summary>
        /// Defines the available types of issues.
        /// </summary>
        public enum InvalidType
        {
            InvalidFrontmatter,
            NoFrontmatter,
            TemplateNotFound,
            NoTemplate
        }

        /// <summary>
        /// Record an issue with the object.
        /// </summary>
        /// <param name="type">Describes the nature of the issue.</param>
        protected void Invalidate(InvalidType type)
        {
            if (!IsInvalid) IsInvalid = true;
            Issues.Add(type);
        }

        protected Item(FileInfo fileInfo)
        {
            Info = fileInfo;
            Link = GetIterablePath(Info);
            Slug = StringService.Slugify(Path.GetFileNameWithoutExtension(Info.Name));
        }

        protected static string GetIterablePath(FileInfo file)
        {
            string relative = Path.GetRelativePath(PathService.GetResourcesPath(), file.FullName);
            string noExtension = Path.ChangeExtension(relative, null);
            string forwardSlashes = noExtension.Replace("\\", "/");
            return $"/{forwardSlashes}/";
        }
    }
}