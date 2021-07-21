using System;
using Dagger.Data.Models;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Display some 'help' text for the Dagger CLI.
    /// </summary>
    public class Help : Routine
    {
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
            Console.WriteLine("- new");
            Console.WriteLine("Creates a new Dagger project in the current directory.");
        }

        public static Routine TooManyArguments(string routineName)
        {
            // Helper method to return a common version of Help.
            return new Help(new Message()
            {
                message = $"{routineName} command received too many arguments.",
                type = Message.Type.Error
            });
        }
    }
}