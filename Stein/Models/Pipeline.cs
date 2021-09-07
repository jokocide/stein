using Stein.Pipelines;

namespace Stein.Models
{
    /// <summary>
    /// Base class for all Pipeline types. A Pipeline is an object type that can evaluate command line arguments.
    /// </summary>
    public abstract class Pipeline
    {
        /// <summary>
        /// The arguments received from the command line are stored here.
        /// </summary>
        protected string[] Arguments { get; }

        protected Pipeline(string[] arguments) => Arguments = arguments;
        
        /// <summary>
        /// Evaluate the arguments provided during object instantiation and return a suitable Routine.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public abstract Routine Execute();

        /// <summary>
        /// Evaluate the given command line arguments and return a suitable Pipeline.
        /// </summary>
        /// <param name="arguments">
        /// Arguments received from the command line.
        /// </param>
        /// <returns>
        /// A Pipeline object that can be used to evaluate command line arguments,
        /// set up the program, and determine which Routine we should be using to
        /// complete the requested action.
        /// </returns>
        public static Pipeline GetPipeline(string[] arguments)
        {
            string argumentZeroLower = arguments[0].ToLower();

            if (argumentZeroLower.Length == 0 || argumentZeroLower == "help") return new HelpPipeline(arguments);
            else if (argumentZeroLower == "build") return new BuildPipeline(arguments);
            else if (argumentZeroLower == "new") return new NewPipeline(arguments);
            else if (argumentZeroLower == "serve") return new ServePipeline(arguments);
            else return new NotRecognizedPipeline(arguments);
        }
    }
}