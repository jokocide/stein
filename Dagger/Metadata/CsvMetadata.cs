using System.IO;

namespace Dagger.Metadata
{
    public class CsvMetadata : Metadata
    {
        public CsvMetadata(FileInfo fileInfo) : base(fileInfo)
        {
        }

        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}