using System;
using System.IO;
using System.Reflection;
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


            foreach (string dir in dirs)
            {
                Console.WriteLine(dir);
            }
            
            Routine instructions = Dispatch.Evaluate(args);

            Console.WriteLine();
            Helper.Colorize(ConsoleColor.Cyan, "Dagger ", false);
            if (args != null)
                Helper.Colorize(ConsoleColor.Gray, String.Join(' ', args));
            Helper.Colorize(ConsoleColor.Cyan, "------");
            
            
            instructions.Execute();
            
            Console.WriteLine();
        }
    }
}
