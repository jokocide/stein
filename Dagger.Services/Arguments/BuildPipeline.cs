using System.IO;
using Dagger.Data.Models;
using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the build command.
    /// </summary>
    public class BuildPipeline : Pipeline
    {
        // The maximum expected arguments for a build command.
        private int MaxBuildArgs { get; } = 2;
        
        public BuildPipeline(string[] args) : base(args) { }
        
        /// <summary>
        /// Returns a Help Routine for invalid commands, or a Build Routine is the command is valid.
        /// </summary>
        /// <returns>
        /// A Routine-typed object.
        /// </returns>
        public override Routine Execute()
        {
            if (Args.Length > MaxBuildArgs) return HelpRoutine.TooManyArguments();

            if (Args.Length == 1)
            {
                if (Helper.CheckIsProject()) return new Build();
                return HelpRoutine.NotInDaggerProject(true);
            }
            
            return PipelineBuildPath(Args);
        }

        /// <summary>
        /// Handle build commands that have received a path argument.
        /// </summary>
        /// <param name="args">
        /// The string[] arguments received from the command line.
        /// </param>
        /// <returns>
        /// A Routine-typed object.
        /// </returns>
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

            return Helper.CheckIsProject() ? new Build() : new HelpRoutine(new Message
            {
                message = "Provide a path to a Dagger project or move to project before calling build.", 
                type = Message.Type.Error
            });
        }
    }
}