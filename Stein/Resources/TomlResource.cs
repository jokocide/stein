using System;
using System.IO;
using Stein.Models;

namespace Stein.Resources
{
    /// <summary>
    /// Represents a TOML file.
    /// </summary>
    public sealed class TomlResource : Resource
    {
        public TomlResource(FileInfo fileInfo) : base(fileInfo) { }

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
            throw new NotImplementedException();
        }
    }
}