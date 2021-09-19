using Stein.Models;

namespace Stein.Interfaces
{
    public interface ISerializable
    {
        public SerializedItem Serialize();
    }
}