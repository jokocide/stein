using System.Collections.Generic;
using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    public class NewPipeline : Pipeline
    {
        public NewPipeline(List<string> args) : base(args) { }
        
        public override Routine Execute()
        {
            // We only support creating a project in the current directory right now, so we should only have one argument.
            return Args.Count > 1 ? Help.TooManyArguments("New") : new New();
        }
    }
}