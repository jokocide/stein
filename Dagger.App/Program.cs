using System;

using Dagger.Services;
using Dagger.Services.Routines;
using Dagger.Services.Arguments;

namespace Dagger.App
{
    /// <summary>
    /// Represents the entry point. Coordinates the execution of routines and services.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // If given arguments, receive a new instance of Routine from argumentsHandler.
            Routine instructions = args.Length > 0 ? Dispatch.Evaluate(args) : new Help();

            // Console header is displayed before the routine is executed.
            Helper.PrintArguments(args);
            
            // Routine is executed, giving the user some feedback.
            instructions.Execute();
            
            // New line is printed for clarity.
            Console.WriteLine();
        }
    }
}
