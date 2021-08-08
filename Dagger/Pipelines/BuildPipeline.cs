using System.IO;
using Dagger.Models;
using Dagger.Routines;
using Dagger.Services;

namespace Dagger.Pipelines
{
    /// <summary>A Pipeline to handle the build command.</summary>
    public sealed class BuildPipeline : Pipeline
    {
        // The maximum expected arguments for a build command.
        private int MaxBuildArgs => 2;

        public BuildPipeline(string[] args) : base(args) { }
        
        /// <summary>Return a Build Routine, or a Help Routine if the command is invalid.</summary>
        /// <returns>A Routine object.</returns>
        public override Routine Execute()
        {
            if (Args.Length > MaxBuildArgs) return HelpRoutine.TooManyArguments();

            if (Args.Length != 1) return PipelineBuildPath(Args);
            return PathService.IsProject() ? new BuildRoutine() : HelpRoutine.NotInDaggerProject(true);
        }

        /// <summary>Handle build commands that have received a path argument.</summary>
        /// <param name="args">The arguments received from the command line.</param>
        /// <returns>A Routine object.</returns>
        private static Routine PipelineBuildPath(string[] args)
        {
            try
            {
                Directory.SetCurrentDirectory(args[1]);
            }
            catch (IOException)
            {
                return HelpRoutine.ProvidedPathIsNotProject();
            }
            
            return PathService.IsProject() ? new BuildRoutine() : HelpRoutine.NotInDaggerProject(true);
        }
    }
}