using Stein.Services;
using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    public abstract class Item
    {
        public string Template { get; set; }

        public string Link { get; set; }

        public string Slug { get; set; }

        public string Date { get; set; }

        public bool IsInvalid { get; private set; }

        public FileInfo Info { get; }

        public List<InvalidType> Issues { get; } = new();

        public void Invalidate(InvalidType type)
        {
            if (!IsInvalid) IsInvalid = true;
            Issues.Add(type);
        }

        public abstract SerializedItem Serialize();

        public enum InvalidType
        {
            InvalidFrontmatter,
            NoFrontmatter,
            TemplateNotFound,
            NoTemplate
        }

        protected Item(FileInfo fileInfo) => Info = fileInfo;

        protected static string GetIterablePath(FileInfo file)
        {
            string relative = Path.GetRelativePath(PathService.ResourcesPath, file.FullName);
            string noExtension = Path.ChangeExtension(relative, null);
            string forwardSlashes = noExtension.Replace("\\", "/");
            return $"/{forwardSlashes}/";
        }
    }
}