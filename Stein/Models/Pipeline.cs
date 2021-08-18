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
    }
}