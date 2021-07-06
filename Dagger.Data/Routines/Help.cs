using System;
using Dagger.Abstract;

namespace Dagger.Routines
{
    public class Help : Routine
    {
        /// <summary>
        /// Display some 'help' text for the Dagger CLI.
        /// </summary>
        private string _message { get; }

        public Help(string message = null)
        {
            _message = message;
        }   
             
        public override void Execute()
        {
            if (_message != null)
            {
                Console.WriteLine(_message);
                Console.WriteLine();
            }

            Console.WriteLine("- build [target]");
            Console.WriteLine("Builds the site that exists at 'target', or builds the current directory if no target is given.");
            Console.WriteLine("- help");
            Console.WriteLine("Display help.");
        }
    }
}