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
            if (Args.Length > 1)
            {
                if (Helper.CheckIsProject(Args[1])) 
                    Directory.SetCurrentDirectory(Args[1]);
                else return HelpRoutine.ProvidedPathIsNotProject();
            }

            if (Helper.CheckIsProject()) 
                return new Watch();
            
            return HelpRoutine.NotInDaggerProject(true);
        }
    }
}