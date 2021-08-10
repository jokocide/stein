using System;
using Dagger.Models;
using Dagger.Services;

namespace Dagger
{
    /// <summary>
    /// Dagger entry point.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Make a call to PipelineService which returns a Routine based on the arguments, and
        /// call the Execute() method defined in that Routine-typed object to perform the desired action.
        /// </summary>
        /// <param name="arguments">The arguments received from the user.</param>
        static void Main(string[] arguments)
        {
            Routine instructions = PipelineService.Evaluate(arguments);
            
            StringService.Colorize(ConsoleColor.Cyan, "Dagger ", false);
            StringService.Colorize(ConsoleColor.Gray, string.Join(' ', arguments));
            StringService.Colorize(ConsoleColor.Cyan, "------");
            
            instructions.Execute();
        }
    }
}
