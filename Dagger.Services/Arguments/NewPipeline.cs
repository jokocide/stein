using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the New command.
    /// </summary>
    public class NewPipeline : Pipeline
    {
        public NewPipeline(string[] args) : base(args) { }
        
        public override Routine Execute()
        {
            // We only support creating a project in the current directory right now, so we should only have one argument.
            return Args.Length > 1 ? HelpRoutine.TooManyArguments() : new New();
        }
    }
}