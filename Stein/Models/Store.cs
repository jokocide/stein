using System.Collections.Generic;

namespace Stein.Models
{
    public class Store
    {
        public void Register<T>(IEnumerable<T> list)
        {
            foreach(T item in list)
            {
                Register(item);
            }
        }

        public void Register<T>(T item)
        {
            switch (item)
            {
                case Writable writable:
                    Register(writable);
                    break;
                case Collection collection:
                    Register(collection);
                    break;
            }
        }

        private void Register(Collection collection) => Collections.Add(collection);

        private void Register(Writable writable) => Writable.Add(writable);

        public List<Collection> Collections { get; } = new();

        public List<Writable> Writable { get; } = new();
    }
}