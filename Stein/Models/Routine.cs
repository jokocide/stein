using Stein.Interfaces;

namespace Stein.Models
{
    public abstract class Routine
    {
        public Routine(IEngine engine, Store store, Configuration config) : this(store, config) => Engine = engine;

        public Routine(Store store, Configuration config) : this(config) => Store = store;

        public Routine(Configuration config) => Config = config;

        public Routine() { }

        public abstract void Execute();

        protected Configuration Config { get; }
    }
}
