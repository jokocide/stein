using System.Collections.Generic;

namespace Stein.Models
{
    public class Store
    {
        public List<Collection> Collections { get; } = new();

        public List<Writable> Writable { get; } = new();
    }
}