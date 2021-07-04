using System;
using Dagger.Abstract;

namespace Dagger.Routines
{
    public class NotRecognized : Routine
    {
        /// <summary>
        /// Triggered when arguments are received but not recognized.
        /// </summary>
        /// <returns>
        /// A string value with instructions for getting to the Dagger 'help' command.
        /// </returns>
        public override void Execute()
        {
            Console.WriteLine("Command not recognized.\nTry reviewing the documentation with 'dagger help' or submit an issue:\nhttps://github.com/jokocide/dagger/issues/new");
        }
    }
}