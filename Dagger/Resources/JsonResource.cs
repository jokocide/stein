using System.IO;
using Dagger.Models;

namespace Dagger.Resources
{
    public sealed class JsonResource : Resource
    {
        internal override Store Data { get; }
        
        public JsonResource(FileInfo fileInfo) : base(fileInfo) { }

        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}