using Stein.Interfaces;

namespace Stein.Models
{
    public abstract class Routine
    {
        public Routine(Configuration config) => Config = config;

        public Routine() { }

        public abstract void Execute();

        protected Configuration Config { get; }
    }
}
