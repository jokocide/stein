using System.IO;
using Stein.Models;
using Stein.Routines;
using Stein.Services;

namespace Stein.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the new command.
    /// </summary>
    public sealed class NewPipeline : Pipeline
    {
        public NewPipeline(string[] args) : base(args) { }

        /// <summary>
        /// Return a NewRoutine, or a HelpRoutine if the command is invalid.
        /// </summary>
        /// <returns>
        /// A Routine object.
        /// </returns>
        public override Routine Execute()
        {
            return Arguments.Length > 1 ? PipelineNewPath(Arguments) : new NewRoutine();
        }

        /// <summary>
        /// Handle new commands that have received a path argument.
        /// </summary>
        /// <param name="arguments">
        /// The arguments received from the command line.
        /// </param>
        /// <returns>
        /// A Routine object.
        /// </returns>
        private Routine PipelineNewPath(string[] arguments)
        {
            if (!Directory.Exists(arguments[1]))
            {
                try
                {
                    Directory.CreateDirectory(arguments[1]);
                }
                catch (IOException)
                {
                    MessageService.Log(Message.ProvidedPathIsInvalid());
                    MessageService.Print(true);
                }
            }

            Directory.SetCurrentDirectory(arguments[1]);
            return new NewRoutine();
        }
    }
}