using System.IO;
using Dagger.Models;

namespace Dagger.Resources
{
    public sealed class XmlResource : Resource
    {
        internal override Store Data { get; }
        
        public XmlResource(FileInfo fileInfo) : base(fileInfo) { }

        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}