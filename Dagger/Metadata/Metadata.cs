using System.Collections.Generic;
using System.IO;

namespace Dagger.Metadata
{
    /// <summary>
    /// Abstract base class for all Metadata types.
    /// </summary>
    public abstract class Metadata
    {
        public FileInfo Info { get; }
        
        public string Template { get; set; }
        
        public string Date { get; set; }

        public List<InvalidType> Issues { get; protected set; }

        protected string ResourcePath =>
            Path.Join(Info.Directory?.Name, Path.GetFileNameWithoutExtension(Info.Name), "index.html");

        protected bool IsInvalid { get; private set; } 

        public abstract void Process();

        protected Metadata(FileInfo fileInfo)
        {
            Info = fileInfo;
        }

        /// <summary>
        /// Invalidate the Metadata and record an InvalidType.
        /// </summary>
        /// <param name="type">An InvalidType that describes a problem with this Metadata.</param>
        protected void Invalidate(InvalidType type)
        {
            if (!IsInvalid) IsInvalid = true;
            Issues.Add(type);
        }

        public enum InvalidType
        {
            Format,
            MissingTemplate
        }
    }
}