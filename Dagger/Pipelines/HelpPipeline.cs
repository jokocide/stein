using Dagger.Routines;

namespace Dagger.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the Help command.
    /// </summary>
    public class HelpPipeline : Pipeline
    {
        public HelpPipeline(string[] args) : base(args) { }
        public override Routine Execute()
        {
            // Todo: More detailed help when a command name is received.
            return new HelpRoutine();
        }
    }
}