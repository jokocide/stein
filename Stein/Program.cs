using Stein.Models;

namespace Stein
{
    /// <summary>
    /// Stein entry point.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Make a call to PipelineService which returns a Routine based on the given arguments, and
        /// call the Execute() method defined in that Routine-typed object to perform the desired action.
        /// </summary>
        /// <param name="arguments">The command line arguments.</param>
        static void Main(string[] arguments)
        {
            // A Pipeline object is capable of evaluating the arguments provided to an acceptable command.
            Pipeline pipeline = Pipeline.GetPipeline(arguments);

            // Use the Pipeline object to find a suitable Routine.
            Routine routine = pipeline.Execute();

            // Execute the routine to complete the requested operation.
            routine.Execute();
        }
    }
}
