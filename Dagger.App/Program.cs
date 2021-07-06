using System;
using System.IO;
using Dagger.Abstract;
using Dagger.Routines;
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
            // Build is the 'default' routine and is called when dagger is invoked with no arguments.
            Routine instructions = new Build();

            // If arguments were received, call ArgumentsHandler and receive a new type of Routine.
            if (args.Length > 0)
            {
                ArgumentsHandler argumentsHandler = new ArgumentsHandler(args);
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
