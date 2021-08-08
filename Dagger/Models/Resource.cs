using System.Collections.Generic;
using System.IO;

namespace Dagger.Models
{
    /// <summary>
    /// Base class for all Resource types.
    /// Represents a file within a Dagger project that can be processed and has the potential to become a Writable.
    /// </summary>
    public abstract class Resource
    {
        /// <summary>
        /// Classes deriving from Resource are responsible for providing a method to retrieve data from that
        /// type of resource.
        /// </summary>
        internal abstract void Process();

        /// <summary>
        /// A resource may have many public properties to expose key pieces of data conveniently, but the Data
        /// property represents the primary storage for all pieces of data derived from a resource.
        /// </summary>
        internal abstract Store Data { get; }
        
        /// <summary>
        /// If an error is encountered during Process() this will be true, indicating something is wrong with the
        /// resource. It will not be fully processed or made into a Writable as a consequence. 
        /// </summary>
        protected bool IsInvalid { get; private set; } 
        
        /// <summary>
        /// A FileInfo object for easy access to this resource's name and directory.
        /// </summary>
        protected FileInfo Info { get; }
         
        /// <summary>
        /// Issues found during Process() are recorded here.
        /// </summary>
        protected List<InvalidType> Issues { get; } = new();

        protected Resource(FileInfo fileInfo) => Info = fileInfo;

        /// <summary>
        /// Invalidate the Metadata with a given InvalidType.
        /// </summary>
        /// <param name="type">An InvalidType to describe the cause of invalidation.</param>
        protected void Invalidate(InvalidType type)
        {
            if (!IsInvalid) IsInvalid = true;
            Issues.Add(type);
        }

        protected enum InvalidType
        {
            InvalidFormat,
            TemplateNotFound
        }
    }
}