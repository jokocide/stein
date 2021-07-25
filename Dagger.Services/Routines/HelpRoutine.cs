using System;
using Dagger.Data.Models;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// A routine that will display a list of all available commands and arguments that Dagger can handle.
    /// </summary>
    public class HelpRoutine : Routine
    {
        // Store any received errors or warnings.
        private Message Message { get; }

        public HelpRoutine(Message message = null)
        {
            Message = message;
        }   
             
        public override void Execute()
        {
            if (Message != null)
            {
                ConsoleColor color = Message.type switch
                {
                    Message.Type.Error => ConsoleColor.Red,
                    Message.Type.Warning => ConsoleColor.Yellow,
                    _ => Console.ForegroundColor
                };

                Helper.Colorize(color, Message.message);
            }

            Console.WriteLine("Optional arguments: ()");
            Console.WriteLine();
            
            Helper.Colorize(ConsoleColor.Cyan, "|> ", false);
            Helper.Colorize(ConsoleColor.Gray, "build ([path])");
            Helper.Colorize(ConsoleColor.DarkGray,
                "Build Dagger project at the provided path, or build the " 
                + "Dagger project in the current directory if no path is provided.");
            Console.WriteLine();
            
            Helper.Colorize(ConsoleColor.Cyan, "|> ", false);
            Helper.Colorize(ConsoleColor.Gray, "new ([path])");
            Helper.Colorize(ConsoleColor.DarkGray,
                "Create a new Dagger project " 
                + "at the given path, or in the current directory if no path is provided.");
            Console.WriteLine();
            
            Helper.Colorize(ConsoleColor.Cyan, "|> ", false);
            Helper.Colorize(ConsoleColor.Gray, "serve ([path]) ([port])");
            Helper.Colorize(ConsoleColor.DarkGray, "Create a local web server with the given [port] and host " 
                                                   + "the Dagger project located at the given [path]. Defaults to " 
                                                   + "port 8000 and checks the current directory for a Dagger " 
                                                   + "project when no arguments are given.");
            Console.WriteLine();
            
            Helper.Colorize(ConsoleColor.Cyan, "|>", false);
            Helper.Colorize(ConsoleColor.Gray, "watch ([path])");
            Helper.Colorize(ConsoleColor.DarkGray, "Watch the Dagger project at the given path for changes, " 
                                                   + "and rebuild the project when a change is detected. Defaults to " 
                                                   + "checking the current directory for a Dagger project when " 
                                                   + "no path is given.");

        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received more than the maximum
        /// number of allowed arguments.
        /// </summary>
        /// <returns>A Routine-typed object.</returns>
        public static Routine TooManyArguments()
        {
            Message message = new Message
            {
                message = "Received more than the maximum number of allowed arguments.",
                type = Message.Type.Error
            };

            return new HelpRoutine(message);
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received a path argument which does
        /// not lead to a Dagger project as indicated by the presence of a hidden .dagger file.
        /// </summary>
        /// <returns>A Routine-typed object.</returns>
        public static Routine ProvidedPathIsNotProject()
        {
            Message message = new Message
            {
                message = "The provided path does not appear to be a Dagger project. (Missing a .dagger file?)",
                type = Message.Type.Error
            };

            return new HelpRoutine(message);
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received a command but cannot proceed
        /// because a path was not provided, and was also not called from the directory of a Dagger project.
        /// </summary>
        /// <returns>A Routine-typed object.</returns>
        public static Routine NotInDaggerProject(bool routineAcceptsPaths)
        {
            string text = routineAcceptsPaths
                ? $"Command was not called from a valid Dagger project directory, and no path was provided."
                : $"Command was not called from a valid Dagger project directory.";

            Message message = new Message
            {
                message = text,
                type = Message.Type.Error
            };
            
            return new HelpRoutine(message);
        }
    }
}