using System.IO;
using Stein.Models;
using Stein.Routines;
using Stein.Services;

namespace Stein.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the build command.
    /// </summary>
    public sealed class BuildPipeline : Pipeline
    {
        /// <summary>
        /// The maximum expected arguments for a build command.
        /// </summary>
        private int MaxBuildArguments => 2;

        public BuildPipeline(string[] arguments) : base(arguments) { }

        /// <summary>
        /// Return a BuildRoutine, or a HelpRoutine if the command is invalid.
        /// </summary>
        /// <returns>
        /// A Routine object.
        /// </returns>
        public override Routine Execute()
        {
            if (Arguments.Length > MaxBuildArguments)
            {
                MessageService.Log(Message.TooManyArguments());
                MessageService.Print(true);
            }

            if (Arguments.Length > 1) return PipelineBuildPath(Arguments);

            if (!PathService.IsProject())
            {
                MessageService.Log(Message.NotInProject(true));
                MessageService.Print(true);
            }

            return new BuildRoutine();
        }

        /// <summary>
        /// Handle build commands that have received a path argument.
        /// </summary>
        /// <param name="arguments">
        /// The arguments received from the command line.
        /// </param>
        /// <returns>
        /// A Routine object.
        /// </returns>
        private static Routine PipelineBuildPath(string[] arguments)
        {
            try
            {
                Directory.SetCurrentDirectory(arguments[1]);
            }
            catch (IOException)
            {
                MessageService.Log(Message.ProvidedPathIsNotProject());
                MessageService.Print(true);
            }

            if (!PathService.IsProject())
            {
                MessageService.Log(Message.NotInProject(true));
                MessageService.Print(true);
            }

            return new BuildRoutine();
        }
    }
}