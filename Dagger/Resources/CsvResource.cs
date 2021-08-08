using System.IO;
using Dagger.Models;

namespace Dagger.Resources
{
    public sealed class CsvResource : Resource
    {
        internal override Store Data { get; }
        
        public CsvResource(FileInfo fileInfo) : base(fileInfo) { }

        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}