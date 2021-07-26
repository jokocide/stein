using System.IO;
using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    public class ServePipeline : Pipeline
    {
        public ServePipeline(string[] args) : base(args) { }

        public override Routine Execute()
        {
            // First argument was serve, did we receive a path?
            if (Args.Length > 1)
            {
                if (Helper.CheckIsProject(Args[1]))
                {
                    Directory.SetCurrentDirectory(Args[1]);
                }
                else
                {
                    return HelpRoutine.ProvidedPathIsNotProject();
                }
            }

            return Helper.CheckIsProject(Directory.GetCurrentDirectory())
                ? new Serve()
                : HelpRoutine.NotInDaggerProject(true);
        }
    }
}