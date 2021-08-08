using System.IO;
using Dagger.Models;

namespace Dagger.Resources
{
    /// <summary>
    /// Represents a JSON file.
    /// </summary>
    public sealed class JsonResource : Resource
    {
        internal override Store Store { get; }
        
        public JsonResource(FileInfo fileInfo) : base(fileInfo) { }

        internal override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}