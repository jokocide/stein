using System;
using System.Collections.Generic;
using System.Linq;
using Stein.Models;

namespace Stein.Services
{
    public static class MessageService
    {
        public static int ErrorCount => Messages.FindAll(item => item.Type == Message.InfoType.Error).Count();

        public static int WarningCount => Messages.FindAll(item => item.Type == Message.InfoType.Warning).Count();

        public static bool HasError => Messages.Any<Message>(item => item.Type == Message.InfoType.Error);

        public static bool HasWarning => Messages.Any<Message>(item => item.Type == Message.InfoType.Warning);

        public static void Log(Message message) => Messages.Add(message);

        public static void Log(Message[] messages) => messages.ToList().ForEach(message => Messages.Add(message));

        public static void Print(Message.InfoType type, bool exit = false)
        {
            Messages.FindAll(message => message.Type == type).ForEach(Print);
            if (exit) Environment.Exit(0);
        }

        public static void Print(bool exit = false)
        {
            Print(Messages);
            if (exit) Environment.Exit(0);
        }

        private static List<Message> Messages { get; } = new();

        private static void Print(IEnumerable<Message> messages) => messages.ToList().ForEach(Print);

        private static void Print(Message message)
        {
            if (message.Type == Message.InfoType.Warning && new ConfigurationService().GetConfigOrNew().SilenceWarnings)
                return;

            if (message.Type == Message.InfoType.Error) StringService.Colorize("Error: ", ConsoleColor.Red, false);
            else if (message.Type == Message.InfoType.Warning) StringService.Colorize("Warning: ", ConsoleColor.Yellow, false);

            StringService.Colorize(message.Text, ConsoleColor.Gray, true);
            Messages.Remove(message);
        }
    }
}