using System;
using System.Collections.Generic;
using System.Linq;
using Stein.Models;

namespace Stein.Services
{
    /// <summary>
    /// Manage issues encountered during runtime.
    /// </summary>
    public static class MessageService
    {
        /// <summary>
        ///  The amount of error messages in memory.
        /// </summary>
        public static int ErrorCount => Messages.FindAll(item => item.Type == Message.InfoType.Error).Count();

        /// <summary>
        /// The amount of warning messages in memory.
        /// </summary>
        public static int WarningCount => Messages.FindAll(item => item.Type == Message.InfoType.Warning).Count();

        /// <summary>
        /// Determine if any Message objects of type Error have been logged.
        /// </summary>
        public static bool HasError => Messages.Any<Message>(item => item.Type == Message.InfoType.Error);

        /// <summary>
        /// Determine if any Message objects of type Warning have been logged.
        /// </summary>
        public static bool HasWarning => Messages.Any<Message>(item => item.Type == Message.InfoType.Warning);

        /// <summary>
        /// Contains the Messages logged during runtime.
        /// </summary>
        private static List<Message> Messages { get; } = new();

        /// <summary>
        /// Log a Message object.
        /// </summary>
        /// <param name="message">the Message object to be recorded.</param>
        public static void Log(Message message)
        {
            Messages.Add(message);
        }

        /// <summary>
        /// Log an array of Message objects.
        /// </summary>
        /// <param name="messages">The Message objects to be recorded.</param>
        public static void Log(Message[] messages)
        {
            messages.ToList().ForEach(message => Messages.Add(message));
        }

        /// <summary>
        /// Print all messages that match the given type, optionally exit the program.
        /// </summary>
        /// <param name="type">The desired message type to search for.</param>
        /// <param name="exit">Determines if the program should exit after printing.</param>
        public static void Print(Message.InfoType type, bool exit = false)
        {
            Messages.FindAll(message => message.Type == type).ForEach(Print);
            if (exit) Environment.Exit(0);
        }

        /// <summary>
        /// Print all messages and optionally exit the program.
        /// </summary>
        /// <param name="exit">Determines if the program should exit after printing.</param>
        public static void Print(bool exit = false)
        {
            Print(Messages);
            if (exit) Environment.Exit(0);
        }

        /// <summary>
        /// Send the given Message objects to stdout.
        /// </summary>
        /// <param name="messages">The Message objects to be printed.</param>
        private static void Print(IEnumerable<Message> messages)
        {
            messages.ToList().ForEach(Print);
        }

        /// <summary>
        /// Send the given Message text to stdout.
        /// </summary>
        /// <param name="message">The Message to be printed.</param>
        private static void Print(Message message)
        {
            // Check configuration to verify that we are supposed to print this message.
            if (message.Type == Message.InfoType.Warning && ConfigurationService.Set.SilenceWarnings)
                return;

            // Display a colored tag to indicate the type of message.
            if (message.Type == Message.InfoType.Error)
            {
                StringService.Colorize("Error: ", ConsoleColor.Red, false);
            }
            else if (message.Type == Message.InfoType.Warning)
            {
                StringService.Colorize("Warning: ", ConsoleColor.Yellow, false);
            }

            // Print the message body in gray font and remove it from memory.
            StringService.Colorize(message.Text, ConsoleColor.Gray, true);
            Messages.Remove(message);
        }
    }
}