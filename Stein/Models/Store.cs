using System.Collections.Generic;

namespace Stein.Models
{
    /// <summary>
    /// Keep track of objects as they are created through runtime and provide methods 
    /// to use those items.
    /// </summary>
    public class Store
    {
        /// <summary>
        /// The registered collections.
        /// </summary>
        public List<Collection> Collections { get; } = new();

        /// <summary>
        /// The fully processed items.
        /// </summary>
        public List<Writable> Writable { get; } = new();
    }
}