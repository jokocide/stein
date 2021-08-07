using System;
using Dagger.Routines;
using Dagger.Services;

namespace Dagger
{
    /// <summary>
    /// Dagger entry point. Coordinates the execution of services.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Get some type of Routine back from Dispatch.
            Routine instructions = PipelineService.Evaluate(args);

            // Print header and arguments.
            StringService.Colorize(ConsoleColor.Cyan, "Dagger ", false);
            if (args != null) StringService.Colorize(ConsoleColor.Gray, string.Join(' ', args));
            StringService.Colorize(ConsoleColor.Cyan, "------");
            
            // Execute routine.
            instructions.Execute();
        }
    }
}
