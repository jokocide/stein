using System;
using Dagger.Models;
using Dagger.Services;

namespace Dagger.Routines
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
                    Message.InfoType.Error => ConsoleColor.Red,
                    Message.InfoType.Warning => ConsoleColor.Yellow,
                    _ => Console.ForegroundColor
                };

                StringService.Colorize(color, Message.Text);
                Console.WriteLine();
            }

            StringService.Colorize(ConsoleColor.Cyan, "|> ", false);
            StringService.Colorize(ConsoleColor.Gray, "build ([path])");
            StringService.Colorize(ConsoleColor.DarkGray,
                "Build Dagger project at the provided path, or build the " 
                + "Dagger project in the current directory if no path is provided.");
            StringService.Colorize(ConsoleColor.Cyan, "|> ", false);
            StringService.Colorize(ConsoleColor.Gray, "new ([path])");
            StringService.Colorize(ConsoleColor.DarkGray,
                "Create a new Dagger project " 
                + "at the given path, or in the current directory if no path is provided.");
            StringService.Colorize(ConsoleColor.Cyan, "|> ", false);
            StringService.Colorize(ConsoleColor.Gray, "serve ([path]) ([port])");
            StringService.Colorize(ConsoleColor.DarkGray, "Create a local web server with the given [port] and host " 
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
            Message message = new()
            {
                Text = "Received too many arguments.",
                Type = Message.InfoType.Error
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
            Message message = new()
            {
                Text = "The provided path does not appear to be a Dagger project. (Missing a .dagger file?)",
                Type = Message.InfoType.Error
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

            Message message = new()
            {
                Text = text,
                Type = Message.InfoType.Error
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
            Message message = new()
            {
                Text = "Command not recognized.",
                Type = Message.InfoType.Error
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
            Message message = new()
            {
                Text = "The provided path does not appear to be valid.",
                Type = Message.InfoType.Error
            };

            return new HelpRoutine(message);
        }
    }
}