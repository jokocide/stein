using System.Collections.Generic;

namespace Stein.Models
{
    class Injectable
    {
        public Dictionary<string, object> Items { get; } = new();

        public static Injectable Assemble(Store store)
        {

        }
    }
}
