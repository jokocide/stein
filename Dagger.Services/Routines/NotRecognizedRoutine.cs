using System;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Report that arguments were received but not understood.
    /// </summary>
    public class NotRecognized : Routine
    {
        public override void Execute()
        {   
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Command not recognized.");
            Console.ResetColor();
            Console.Write("Get help with 'dagger help' or submit an issue: ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("https://github.com/jokocide/dagger/issues/new");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}