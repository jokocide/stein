using System.IO;

namespace Dagger.Metadata
{
    public class XmlMetadata : Metadata
    {
        public XmlMetadata(FileInfo fileInfo) : base(fileInfo)
        {
        }

        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}