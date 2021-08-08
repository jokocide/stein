using System.IO;
using Dagger.Models;
using Dagger.Routines;
using Dagger.Services;

namespace Dagger.Pipelines
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
        /// <returns>A Routine object.</returns>
        public override Routine Execute()
        {
            if (Arguments.Length > MaxBuildArguments) return HelpRoutine.TooManyArguments();

            if (Arguments.Length != 1) return PipelineBuildPath(Arguments);
            return PathService.IsProject() ? new BuildRoutine() : HelpRoutine.NotInDaggerProject(true);
        }

        /// <summary>
        /// Handle build commands that have received a path argument.
        /// </summary>
        /// <param name="arguments">The arguments received from the command line.</param>
        /// <returns>A Routine object.</returns>
        private static Routine PipelineBuildPath(string[] arguments)
        {
            try
            {
                Directory.SetCurrentDirectory(arguments[1]);
            }
            catch (IOException)
            {
                return HelpRoutine.ProvidedPathIsNotProject();
            }
            
            return PathService.IsProject() ? new BuildRoutine() : HelpRoutine.NotInDaggerProject(true);
        }
    }
}