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
            Dispatch argumentsHandler = new Dispatch(args);
            
            /*
            * If arguments were received, call ArgumentsHandler and receive a new type of Routine.
            * Help is the 'default' routine and is called when dagger is invoked with no arguments.
            */
            Routine instructions = args.Length > 0 ? argumentsHandler.Evaluate() : new Help();

            Helper.PrintArguments(args);
            instructions.Execute();
            Console.WriteLine();
        }
    }
}
