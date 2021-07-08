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
            // Help is the 'default' routine and is called when dagger is invoked with no arguments.
            Routine instructions = new Help();

            // If arguments were received, call ArgumentsHandler and receive a new type of Routine.
            if (args.Length > 0)
            {
                Dispatch argumentsHandler = new Dispatch(args);
                instructions = argumentsHandler.Evaluate();
            }
 
            // Helper prints out the arguments that were received to make the output more readable.
            Helper.PrintArguments(args);

            // After determining which Routine to use, call the Execute method.
            instructions.Execute();

            // Most Routine types output text to the console, so an empty line here makes it more readable.
            Console.WriteLine();
        }
    }
}
