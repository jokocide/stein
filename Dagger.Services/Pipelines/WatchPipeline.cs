using System.Collections.Generic;
using System.IO;

using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    public class WatchPipeline : Pipeline
    {
        public WatchPipeline(List<string> args) : base(args) {}

        public override Routine Execute()
        {
            if (Args.Count > 1)
            {
                if (Helper.CheckIsProject(Args[1])) 
                    Directory.SetCurrentDirectory(Args[1]);
                else return Help.ProvidedPathIsNotProject(Args[1]);
            }

            if (Helper.CheckIsProject()) 
                return new Watch();
            
            return Help.NotInDaggerProject(true);
        }
    }
}