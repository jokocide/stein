using System;
using Dagger.Abstract;
using Dagger.Data.Models;

namespace Dagger.Routines
{
    public class Help : Routine
    {
        /// <summary>
        /// Display some 'help' text for the Dagger CLI.
        /// </summary>
        private Message _message { get; }

        public Help(Message message = null)
        {
            _message = message;
        }   
             
        public override void Execute()
        {
            if (_message != null)
            {
                
                if (_message.type == Message.Type.Error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (_message.type == Message.Type.Warning)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.WriteLine(_message.message);
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.WriteLine("- build [PATH]");
            Console.WriteLine("Builds the site that exists at the given path, or builds the current directory if no target is given.");
            // Console.WriteLine("- help");
            // Console.WriteLine("Display help.");
        }
    }
}