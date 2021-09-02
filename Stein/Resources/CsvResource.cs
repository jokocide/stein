using System;
using System.IO;
using Stein.Models;

namespace Stein.Resources
{
    /// <summary>
    /// Represents a CSV file.
    /// </summary>
    public sealed class CsvResource : Resource
    {
        public CsvResource(FileInfo fileInfo) : base(fileInfo) { }

        /// <summary>
        /// Return all data in a format suitable for template injection.
        /// </summary>
        /// <returns></returns>
        internal override Injectable Serialize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Populate the properties of this Resource.
        /// </summary>
        internal override void Process(Store store)
        {
            throw new System.NotImplementedException();
        }
    }
}