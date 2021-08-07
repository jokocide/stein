using System.IO;

namespace Dagger.Metadata
{
    public class JsonMetadata : Metadata
    {
        public JsonMetadata(FileInfo fileInfo) : base(fileInfo)
        {
        }

        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}