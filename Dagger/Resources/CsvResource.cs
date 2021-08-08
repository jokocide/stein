using System.IO;
using Dagger.Models;

namespace Dagger.Resources
{
    /// <summary>
    /// Represents a CSV file.
    /// </summary>
    public sealed class CsvResource : Resource
    {
        internal override Store Store { get; }
        
        public CsvResource(FileInfo fileInfo) : base(fileInfo) { }

        internal override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}