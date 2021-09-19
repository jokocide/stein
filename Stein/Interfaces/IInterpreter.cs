using System.Collections.Generic;

namespace Stein.Interfaces
{
    public interface IInterpreter
    {
        public Dictionary<string, string> Deserialize(string text);
    }
}
