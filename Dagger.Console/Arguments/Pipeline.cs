using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    /// <summary>
    /// Abstract base class for other Pipeline types to derive from.
    /// </summary>
    public abstract class Pipeline
    {
        protected string[] Args { get; }

        protected Pipeline(string[] args)
        {
            Args = args;
        }
        
        public abstract Routine Execute();
    }
}