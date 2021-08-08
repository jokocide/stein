using System.IO;
using Dagger.Models;

namespace Dagger.Resources
{
    public sealed class TomlResource : Resource
    {
        internal override Store Data { get; }

        public TomlResource(FileInfo fileInfo) : base(fileInfo) { }

        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}