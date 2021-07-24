using System.Collections.Generic;
using System.IO;
using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    public class ServePipeline : Pipeline
    {
        public ServePipeline(List<string> args) : base(args) { }

        public override Routine Execute()
        {
            // First argument was serve, did we receive a path?
            if (Args.Count > 1 && Helper.CheckIsProject(Args[1])) 
                Directory.SetCurrentDirectory(Path.Join(Args[1]));

            return Helper.CheckIsProject(Directory.GetCurrentDirectory())
                ? new Serve()
                : Help.NotInDaggerProject(true);
        }
    }
}