using Stein.Models;
using Stein.Pipelines;
using Stein.Routines;

namespace Stein.Services
{
    /// <summary>
    /// Provides a method to handle command line arguments.
    /// </summary>
    /// <returns>
    /// A Routine object.
    /// </returns>
    public static class PipelineService
    {
        /// <summary>
        /// Dicate the maximum allowed arguments across all possible combinations of
        /// acceptable commands.
        /// </summary>
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
            if (arguments.Length == 0) return new HelpRoutine();
            
            // We can return right away if too many arguments are passed in.
            if (arguments.Length > MaxTotalArgs)
            {
                MessageService.Log(Message.TooManyArguments());
                MessageService.Print(true);
            }

            Routine routine = null;

            // Attempt to populate routine with a Routine type object.
            switch (arguments[0].ToLower())
            {
                case "help":
                    routine = new HelpPipeline(arguments).Execute();
                    break;
                case "build":
                    routine = new BuildPipeline(arguments).Execute();
                    break;
                case "new":
                    routine = new NewPipeline(arguments).Execute();
                    break;
                case "serve":
                    routine = new ServePipeline(arguments).Execute();
                    break;
                default:
                    MessageService.Log(Message.CommandNotRecognized());
                    MessageService.Print(true);
                    break;
            }

            return routine;
        }
    }
}