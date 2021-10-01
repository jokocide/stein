using Stein.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stein.Models
{
    public class Message
    {
        public Message(string text, InfoType type)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; init; }

        public InfoType Type { get; init; }

        public static Message TooManyArgs() => new("Received too many arguments.", InfoType.Error);

        public static Message NoTemplateKey(FileInfo fileInfo) => new($"No template key: {fileInfo.Name}", InfoType.Warning);

        public static Message ProvidedPathIsNotProject() => new("The provided path does not appear to be a Stein project. (Missing a stein.json file?)", InfoType.Error);

        public static Message NotInProject(bool routineAcceptsPaths)
        {
            string text = routineAcceptsPaths
                ? $"Command was not called from a valid Stein project directory, and no path was provided."
                : $"Command was not called from a valid Stein project directory.";

            Message message = new(text, Message.InfoType.Error);
            return message;
        }

        public static Message NoEngine() => new("A recognized engine (hbs) is not specified in stein.json.", InfoType.Error);

        public static Message CommandNotRecognized() => new("Command not recognized.", InfoType.Error);

        public static Message ProvidedPathIsInvalid() => new("The provided path does not appear to be valid.", InfoType.Error);

        public static Message NoExtension(FileInfo fileInfo) => new($"File has no extension: {fileInfo.Name}", InfoType.Error);

        public static Message InvalidJson(FileInfo fileInfo) => new($"Invalid JSON: {fileInfo.Name}", InfoType.Error);

        public static Message ProjectAlreadyExists() => new("A stein.json already exists in target directory.", Message.InfoType.Error);

        public static Message InvalidConfiguration() => new($"Invalid project configuration in stein.json.", InfoType.Error);

        public static void Log(Message message) => Messages.Add(message);

        public static void Print(bool exit = false)
        {
            Print(Messages);
            if (exit) Environment.Exit(0);
        }

        public enum InfoType
        {
            Warning,
            Error
        }

        private static List<Message> Messages { get; } = new();

        private static void Print(IEnumerable<Message> messages) => messages.ToList().ForEach(Print);

        private static void Print(Message message)
        {
            if (message.Type == Message.InfoType.Warning && new Configuration().GetConfig().SilenceWarnings)
                return;

            if (message.Type == Message.InfoType.Error) StringService.Colorize("Error: ", ConsoleColor.Red, false);
            else if (message.Type == Message.InfoType.Warning) StringService.Colorize("Warning: ", ConsoleColor.Yellow, false);

            StringService.Colorize(message.Text, ConsoleColor.Gray, true);
            Messages.Remove(message);
        }
    }
}