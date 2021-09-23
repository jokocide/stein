using System.Collections.Generic;

namespace Stein.Interfaces
{
    public interface IDeserializer
    {
        public Dictionary<string, string> Deserialize(string text);
    }
}
