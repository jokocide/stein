using System.IO;

namespace Stein.Models
{
    public abstract class Template
    {
        public Template(FileInfo fileInfo, object templateObject) : this(fileInfo) => TemplateObject = templateObject;

        public Template(FileInfo fileInfo) => Info = fileInfo;

        public FileInfo Info { get; }

        public object TemplateObject { get; }
    }
}