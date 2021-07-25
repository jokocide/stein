using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    public class HelpPipeline : Pipeline
    {
        public HelpPipeline(string[] args) : base(args) { }
        public override Routine Execute()
        {
            // todo: Maybe display more detailed help when a routine name is passed in?
            return new HelpRoutine();
        }
    }
}