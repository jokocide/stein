using Stein.Interfaces;

namespace Stein.Models
{
    public class Routine
    {
        public Routine(IEngine engine, Store store, Configuration config) : this(store, config) => Engine = engine;

        public Routine(Store store, Configuration config) : this(config) => Store = store;

        public Routine(Configuration config) => Config = config;

        protected IEngine Engine { get; }

        protected Store Store { get; }

        protected Configuration Config { get; }
    }
}
