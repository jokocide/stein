namespace Stein.Models
{
    public class Routine
    {
        public Routine(Store store, Configuration config)
        {
            Store = store;
            Config = config;
        }

        public Routine(Configuration config) => Config = config;

        protected Store Store { get; }

        protected Configuration Config { get; }
    }
}
