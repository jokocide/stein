using System;
using Dagger.Data.Models;

namespace Dagger.Services.Routines
{
    /// <summary>
    /// Display help documentation for the CLI.
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
                ConsoleColor color = Message.Type switch
                {
                    Message.MessageType.Error => ConsoleColor.Red,
                    Message.MessageType.Warning => ConsoleColor.Yellow,
                    _ => Console.ForegroundColor
                };

                Helper.Colorize(color, Message.Text);
                Console.WriteLine();
            }

            Helper.Colorize(ConsoleColor.Cyan, "|> ", false);
            Helper.Colorize(ConsoleColor.Gray, "build ([path])");
            Helper.Colorize(ConsoleColor.DarkGray,
                "Build Dagger project at the provided path, or build the " 
                + "Dagger project in the current directory if no path is provided.");
            Helper.Colorize(ConsoleColor.Cyan, "|> ", false);
            Helper.Colorize(ConsoleColor.Gray, "new ([path])");
            Helper.Colorize(ConsoleColor.DarkGray,
                "Create a new Dagger project " 
                + "at the given path, or in the current directory if no path is provided.");
            Helper.Colorize(ConsoleColor.Cyan, "|> ", false);
            Helper.Colorize(ConsoleColor.Gray, "serve ([path]) ([port])");
            Helper.Colorize(ConsoleColor.DarkGray, "Create a local web server with the given [port] and host " 
                                                   + "the Dagger project located at the given [path]. Defaults to " 
                                                   + "port 8000 and checks the current directory for a Dagger " 
                                                   + "project when no arguments are given.");
            Console.WriteLine();
            
            /*
             * It is possible that another called routine will instantiate and call this Execute method,
             * and for that reason we need to explicitly exit the program here so that we don't continue
             * execution in the previous context.
             */
            Environment.Exit(0);
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received more than the maximum
        /// number of allowed arguments.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Routine TooManyArguments()
        {
            Message message = new Message
            {
                Text = "Received more than the maximum number of allowed arguments.",
                Type = Message.MessageType.Error
            };

            return new HelpRoutine(message);
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received a path argument which does
        /// not lead to a Dagger project as indicated by the presence of a hidden .dagger file.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Routine ProvidedPathIsNotProject()
        {
            Message message = new Message
            {
                Text = "The provided path does not appear to be a Dagger project. (Missing a .dagger file?)",
                Type = Message.MessageType.Error
            };

            return new HelpRoutine(message);
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received a command but cannot proceed
        /// because a path was not provided, and was also not called from the directory of a Dagger project.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Routine NotInDaggerProject(bool routineAcceptsPaths)
        {
            string text = routineAcceptsPaths
                ? $"Command was not called from a valid Dagger project directory, and no path was provided."
                : $"Command was not called from a valid Dagger project directory.";

            Message message = new Message
            {
                Text = text,
                Type = Message.MessageType.Error
            };
            
            return new HelpRoutine(message);
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received arguments but does not
        /// understand how to respond to them.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Routine CommandNotRecognized()
        {
            Message message = new Message
            {
                Text = "Command not recognized.",
                Type = Message.MessageType.Error
            };

            return new HelpRoutine(message);
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger received a path argument, but
        /// the path argument does not appear to be valid. Dagger cannot create or move into the directory.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Routine ProvidedPathIsInvalid()
        {
            Message message = new Message
            {
                Text = "The provided path does not appear to be valid.",
                Type = Message.MessageType.Error
            };

            return new HelpRoutine(message);
        }
    }
}