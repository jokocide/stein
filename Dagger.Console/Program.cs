using System;
using System.IO;
using System.Reflection;
using Dagger.Services;
using Dagger.Services.Routines;
using Dagger.Services.Arguments;

namespace Dagger.App
{
    /// <summary>
    /// Dagger entry point. Coordinates the execution of services.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Get some type of Routine back from Dispatch.
            Routine instructions = Dispatch.Evaluate(args);

            // Print header and arguments.
            Helper.Colorize(ConsoleColor.Cyan, "Dagger ", false);
            if (args != null) Helper.Colorize(ConsoleColor.Gray, string.Join(' ', args));
            Helper.Colorize(ConsoleColor.Cyan, "------");
            
            // Execute routine.
            instructions.Execute();
        }
    }
}
