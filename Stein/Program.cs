using Stein.Models;
using Stein.Services;

namespace Stein
{
    /// <summary>Stein entry point.</summary>
    class Program
    {
        /// <summary>
        /// Make a call to PipelineService which returns a Routine based on the given arguments, and
        /// call the Execute() method defined in that Routine-typed object to perform the desired action.
        /// </summary>
        /// <param name="arguments">The command line arguments.</param>
        static void Main(string[] arguments)
        {
            Routine instructions = PipelineService.Evaluate(arguments);
            instructions.Execute();
        }
    }
}
