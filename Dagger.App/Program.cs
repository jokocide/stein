using System;
using Dagger.Services.Routines;
using Dagger.Services;

namespace Dagger.App
{
    /// <summary>
    /// Program.cs represents the entry point of the application and serves as a 
    /// 'middleware' chain to coordinate the execution of routines and services.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // If given arguments, receive a new instance of Routine from argumentsHandler.
            Routine instructions = args.Length > 0 ? Dispatch.Evaluate(args) : new Help();

            // Header is printed out, routine is executed.
            Helper.PrintArguments(args);
            
            instructions.Execute();
            
            // New line for clarity!
            Console.WriteLine();
        }
    }
}
