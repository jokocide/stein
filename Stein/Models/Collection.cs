using System.Collections.Generic;
using System.IO;

namespace Stein.Models
{
    /// <summary>
    /// Represents a group of Resource objects from the same origin.
    /// </summary>
    public class Collection
    {
        /// <summary>
        /// A DirectoryInfo from the collection directory.
        /// </summary>
        public DirectoryInfo Info { get; }

        /// <summary>
        /// The Resource objects that are a part of this collection.
        /// </summary>
        public List<Resource> Items { get; } = new();
        
        public Collection(DirectoryInfo directoryInfo) => Info = directoryInfo;
    }
}