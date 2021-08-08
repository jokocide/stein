using System;
using System.IO;
using Dagger.Models;
using Dagger.Routines;
using Dagger.Services;

namespace Dagger.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the Serve command.
    /// </summary>
    public sealed class ServePipeline : Pipeline
    {
        public ServePipeline(string[] arguments) : base(arguments) { }

        /// <summary>
        /// Return a ServeRoutine, or a HelpRoutine if the command is invalid.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public override Routine Execute()
        {
            if (Arguments.Length > 1) return ServePathPipeline(Arguments);

            return PathService.IsProject(Directory.GetCurrentDirectory())
                ? new ServeRoutine()
                : HelpRoutine.NotInDaggerProject(true);
        }

        /// <summary>
        /// Handle serve commands that have received a path argument.
        /// </summary>
        /// <param name="arguments">The arguments received from the command line.</param>
        /// <returns>A Routine object.</returns>
        private Routine ServePathPipeline(string[] arguments)
        {
            if (arguments.Length > 2) return ServePathPortPipeline(arguments);
            
            if (!PathService.IsProject(arguments[1]))
            {
                int number;
                
                if (Int32.TryParse(arguments[1], out number))
                {
                    return new ServeRoutine(arguments[1]);
                }

                return HelpRoutine.ProvidedPathIsNotProject();
            }
            
            Directory.SetCurrentDirectory(arguments[1]);
            return new ServeRoutine();
        }

        /// <summary>
        /// Handle serve commands that have received a path and port argument.
        /// </summary>
        /// <param name="arguments">The arguments received from the command line.</param>
        /// <returns>A Routine object.</returns>
        private Routine ServePathPortPipeline(string[] arguments)
        {
            if (!PathService.IsProject(arguments[1])) return HelpRoutine.ProvidedPathIsNotProject();
            
            Directory.SetCurrentDirectory(arguments[1]);
            return new ServeRoutine(arguments[2]);
        }
    }
}