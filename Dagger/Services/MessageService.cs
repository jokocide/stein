using System;
using System.Collections.Generic;
using System.Linq;
using Dagger.Models;

namespace Dagger.Services
{
    /// <summary>
    /// Manage issues encountered during runtime.
    /// </summary>
    public class MessageService
    {
        /// <summary>
        /// Contains the Messages logged during runtime.
        /// </summary>
        private static List<Message> Messages { get; } = new();

        public static void Log(Message message)
        {
            Messages.Add(message);
        }

        public static void Log(Message[] messages)
        {
            messages.ToList().ForEach(message => Messages.Add(message));
        }
        
        /// <summary>
        /// Report all messages that match the given type, optionally exit the program.
        /// </summary>
        /// <param name="type">
        /// The desired message type to search for.
        /// </param>
        /// <param name="exit">
        /// Determines if the program should exit after printing.
        /// </param>
        public void Print(Message.InfoType type, bool exit = false)
        {
            Messages.FindAll(message => message.Type == type).ForEach(Print);
            Environment.Exit(1);
        }

        /// <summary>
        /// Report all messages, optionally exit the program.
        /// </summary>
        /// <param name="exit">
        /// Determines if the program should exit after printing.
        /// </param>
        public void Print(bool exit = false)
        {
            Print(Messages);
        }

        /// <summary>
        /// Send the given Message text to stdout.
        /// </summary>
        /// <param name="message">The Message to be printed.</param>
        private void Print(Message message)
        {
            Console.WriteLine(message.Text);
        }

        /// <summary>
        /// Send the given Message objects to stdout.
        /// </summary>
        /// <param name="messages">The Message objects to be printed.</param>
        private void Print(IEnumerable<Message> messages)
        {
            messages.ToList().ForEach(Print);
        }
    }
}