using System.Collections.Generic;
using System.IO;

namespace Dagger.Models
{
    /// <summary>
    /// Represents a group of Metadata objects from the same origin.
    /// </summary>
    public class Collection
    {
        public DirectoryInfo Info { get; }

        public List<Metadata.Metadata> Items { get; } = new();
        
        public Collection(string path)
        {
            if (Directory.Exists(path)) Info = new DirectoryInfo(path);
        }

        public Collection(DirectoryInfo directoryInfo)
        {
            if (Directory.Exists(directoryInfo.FullName)) Info = directoryInfo;
        }
    }
}