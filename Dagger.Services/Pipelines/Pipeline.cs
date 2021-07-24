using System.Collections.Generic;
using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    /// <summary>
    /// Abstract class for other piplines to derive from.
    /// </summary>
    public abstract class Pipeline
    {
        protected List<string> Args { get; }

        protected Pipeline(List<string> args)
        {
            Args = args;
        }
        
        public abstract Routine Execute();
    }
}