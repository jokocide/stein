using System.IO;
using Dagger.Models;

namespace Dagger.Resources
{
    /// <summary>
    /// Represents a TOML file.
    /// </summary>
    public sealed class TomlResource : Resource
    {
        internal override Store Store { get; }

        public TomlResource(FileInfo fileInfo) : base(fileInfo) { }

        internal override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}