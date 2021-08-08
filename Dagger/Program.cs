using System;
using Dagger.Models;
using Dagger.Services;

namespace Dagger
{
    /// <summary>
    /// Dagger entry point. Coordinates the execution of services.
    /// </summary>
    class Program
    {
        static void Main(string[] arguments)
        {
            // Get some type of Routine back from Dispatch.
            Routine instructions = PipelineService.Evaluate(arguments);

            // Print header and arguments.
            StringService.Colorize(ConsoleColor.Cyan, "Dagger ", false);
            if (arguments != null) StringService.Colorize(ConsoleColor.Gray, string.Join(' ', arguments));
            StringService.Colorize(ConsoleColor.Cyan, "------");
            
            // Execute routine.
            instructions.Execute();
        }
    }
}
