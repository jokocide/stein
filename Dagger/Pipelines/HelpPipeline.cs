using Dagger.Models;
using Dagger.Routines;

namespace Dagger.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the help command.
    /// </summary>
    public sealed class HelpPipeline : Pipeline
    {
        public HelpPipeline(string[] arguments) : base(arguments) { }
        
        /// <summary>
        /// Return a HelpRoutine.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public override Routine Execute()
        {
            // Todo: More detailed help when a command name is received.
            return new HelpRoutine();
        }
    }
}