using System.IO;
using Dagger.Models;
using Dagger.Pipelines;
using Dagger.Routines;

namespace Dagger.Services
{
    /// <summary>
    /// Provides a method to handle command line arguments.
    /// </summary>
    /// <returns>
    /// A Routine object.
    /// </returns>
    public static class PipelineService
    {
        private static int MaxTotalArgs => 3;

        /// <summary>
        /// Take in the received command line arguments and return a Routine based on user input.
        /// </summary>
        /// <param name="arguments">
        /// All of the command line arguments that were received from the user.
        /// </param>
        /// <returns>
        /// A Routine object best suited to respond to the given arguments.
        /// </returns>
        public static Routine Evaluate(string[] arguments)
        {
            // If no arguments are received, serve the current directory.
            if (arguments.Length == 0)
            {
                return !PathService.IsProject(Directory.GetCurrentDirectory())
                    ? HelpRoutine.NotInDaggerProject(false)
                    : new ServeRoutine();
            }
            
            // We can return right away if too many arguments are passed in.
            if (arguments.Length > MaxTotalArgs) return HelpRoutine.TooManyArguments();

            return arguments[0].ToLower() switch
            {
                "help" => new HelpPipeline(arguments).Execute(),
                "build" => new BuildPipeline(arguments).Execute(),
                "new" => new NewPipeline(arguments).Execute(),
                "serve" => new ServePipeline(arguments).Execute(),
                _ => HelpRoutine.CommandNotRecognized()
            };
        }
    }
}