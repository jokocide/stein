using System.IO;

namespace Dagger.Metadata
{
    public class TomlMetadata : Metadata
    {
        public TomlMetadata(FileInfo fileInfo) : base(fileInfo)
        {
        }

        public override void Process()
        {
            throw new System.NotImplementedException();
        }
    }
}