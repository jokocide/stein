using System.IO;
using Dagger.Models;

namespace Dagger.Resources
{
    /// <summary>
    /// Represents an XML file.
    /// </summary>
    public sealed class XmlResource : Resource
    {
        internal override Store Store { get; }
        
        public XmlResource(FileInfo fileInfo) : base(fileInfo) { }

        internal override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}