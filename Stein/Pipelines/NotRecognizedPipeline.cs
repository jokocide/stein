using Stein.Models;
using Stein.Routines;

namespace Stein.Pipelines
{
    /// <summary>
    /// A Pipeline to handle commands that are not recognized.
    /// </summary>
    public sealed class NotRecognizedPipeline : Pipeline
    {
        public NotRecognizedPipeline(string[] arguments) : base(arguments) { }

        /// <summary>
        /// Return a NotRecognizedRoutine.
        /// </summary>
        /// <returns>
        /// A Routine object.
        /// </returns>
        public override Routine Execute()
        {
            return new NotRecognizedRoutine();
        }
    }
}
