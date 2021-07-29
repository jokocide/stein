using System.Collections.Generic;
using System.IO;

using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the Watch command.
    /// </summary>
    public class WatchPipeline : Pipeline
    {
        public WatchPipeline(string[] args) : base(args) {}

        public override Routine Execute()
        {
            if (Args.Length > 1) WatchPathPipeline(Args);
            if (Helper.CheckIsProject()) return new WatchRoutine();
            
            return HelpRoutine.NotInDaggerProject(true);
        }

        private Routine WatchPathPipeline(string[] args)
        {
            if (!Helper.CheckIsProject(args[1])) return HelpRoutine.ProvidedPathIsNotProject();
            Directory.SetCurrentDirectory(args[1]);
            
            return new WatchRoutine();
        }
    }
}