using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the Help command.
    /// </summary>
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