using System;

using Dagger.Data.Models;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Display some 'help' text for the Dagger CLI.
    /// </summary>
    public class Help : Routine
    {
        private Message Message { get; }

        public Help(Message message = null)
        {
            Message = message;
        }   
             
        public override void Execute()
        {
            if (Message != null)
            {
                if (Message.type == Message.Type.Error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (Message.type == Message.Type.Warning)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                Console.WriteLine(Message.message);
                Console.ResetColor();
            }

            Console.WriteLine("Optional arguments: ()");
            Console.WriteLine();
            Helper.Colorize(ConsoleColor.Cyan, "|> ", false);
            Helper.Colorize(ConsoleColor.Gray, "build ([path])");
            Helper.Colorize(ConsoleColor.DarkGray,
                "Build Dagger project at the given path, or build the Dagger project in the current directory when no path is given.");
            Console.WriteLine();
            Helper.Colorize(ConsoleColor.Cyan, "|> ", false);
            Helper.Colorize(ConsoleColor.Gray, "new");
            Helper.Colorize(ConsoleColor.DarkGray, "Create a new Dagger project in the current directory.");
        }

        public static Routine TooManyArguments(string routineName = null)
        {
            string start = routineName == null ? "Dagger " : $"{routineName} routine ";
            
            return new Help(new Message()
            {
                message = start + "received too many arguments.",
                type = Message.Type.Error
            });
        }

        public static Routine ProvidedPathIsNotProject(string routineName)
        {
            return new Help(new Message()
            {
                message =
                    $"{routineName} routine received a path, but the path doesn't appear to represent a Dagger project. (missing .dagger file?)",
                type = Message.Type.Error
            });
        }

        public static Routine NotInDaggerProject(bool routineAcceptsPaths)
        {
            string text = routineAcceptsPaths
                ? $"Move to a Dagger project directory or provide a path."
                : $"Move to a Dagger project directory.";
            
            return new Help(new Message() { message = text, type = Message.Type.Error });
        }
    }
}