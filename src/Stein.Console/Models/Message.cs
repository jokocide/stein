using Stein.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Stein.Models
{
    /// <summary>
    /// Represents an event that has occurred.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Initializes a new instance of the Message class with the given text and type.
        /// </summary>
        /// <param name="text">The text that will be displayed to the user.</param>
        /// <param name="type">The severity of the message.</param>
        public Message(string text, InfoType type)
        {
            Text = text;
            Type = type;
        }

        /// <summary>
        /// Contains the text that will be displayed to the user.
        /// </summary>
        public string Text { get; init; }

        /// <summary>
        /// Contains the severity of the message.
        /// </summary>
        public InfoType Type { get; init; }

        /// <summary>
        /// Return a new instance of the Message class to indicate that the program has received too many arguments.
        /// </summary>
        public static Message TooManyArgs() => new("Received too many arguments.", InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that the file does not contain a 
        /// template key.
        /// </summary>
        /// <param name="fileInfo">A FileInfo object derived from the file.</param>
        public static Message NoTemplateKey(FileInfo fileInfo) => new($"No template key: {fileInfo.Name}", InfoType.Warning);

        /// <summary>
        /// Return a new instance of the Message class to indicate that the template is invalid.
        /// </summary>
        /// <param name="fileInfo">A FileInfo object derived from the template file.</param>
        public static Message InvalidTemplate(FileInfo fileInfo) => new($"Invalid template: {fileInfo.Name}", InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that the path provided to the routine does
        /// not lead to a valid project.
        /// </summary>
        public static Message ProvidedPathIsNotProject() => new("The provided path does not appear to be a Stein project. (Missing a stein.json file?)", InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that a command was given outside of a valid project.
        /// </summary>
        /// <param name="routineAcceptsPaths">
        /// Affects the final text of the message based tolerance for paths.
        /// </param>
        public static Message NotInProject(bool routineAcceptsPaths)
        {
            string text = routineAcceptsPaths
                ? $"Command was not called from a valid Stein project directory, and no path was provided."
                : $"Command was not called from a valid Stein project directory.";

            Message message = new(text, Message.InfoType.Error);
            return message;
        }

        /// <summary>
        /// Return a new instance of the Message class to indicate that an engine has not been specified within
        /// the project configuration file.
        /// </summary>
        public static Message NoEngine() => new("A recognized engine (hbs) is not specified in stein.json.", InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that a command is not recognized.
        /// </summary>
        public static Message CommandNotRecognized() => new("Command not recognized.", InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that the provided path is invalid.
        /// </summary>
        public static Message ProvidedPathIsInvalid() => new("The provided path does not appear to be valid.", InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate the the file does not have an extension.
        /// </summary>
        /// <param name="fileInfo">A FileInfo object derived from the file.</param>
        public static Message NoExtension(FileInfo fileInfo) => new($"File has no extension: {fileInfo.Name}", InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that the file contains invalid JSON.
        /// </summary>
        /// <param name="fileInfo">A FileInfo object derived from the file.</param>
        public static Message InvalidJson(FileInfo fileInfo) => new($"Invalid JSON: {fileInfo.Name}", InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that the file does not appear to contain YAML frontmatter.
        /// </summary>
        /// <param name="fileInfo">A FileInfo object derived from the file.</param>
        public static Message NoYaml(FileInfo fileInfo) => new Message($"No YAML: {fileInfo.Name}", InfoType.Warning);

        /// <summary>
        /// Return a new instance of the Message class to indicate that the directory already appears to be
        /// an initialized project, because it contains a stein.json file.
        /// </summary>
        public static Message ProjectAlreadyExists() => new("A stein.json already exists in target directory.", Message.InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that the file contains an invalid key/value pair within 
        /// the YAML frontmatter.
        /// </summary>
        /// <param name="fileInfo">A FileInfo object derived from the file.</param>
        public static Message InvalidKeyValuePair(FileInfo fileInfo) => new($"Invalid key/value pair in YAML: {fileInfo.Name}", Message.InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that the project configuration appears to be invalid.
        /// </summary>
        public static Message InvalidConfiguration() => new($"Invalid project configuration in stein.json.", InfoType.Error);

        /// <summary>
        /// Return a new instance of the Message class to indicate that a restart of the ServeRoutine is required.
        /// </summary>
        /// <param name="args">Arguments derived from the exception.</param>
        public static Message ServerRestartRequired(ErrorEventArgs args) => new($"A server restart is required: ({args.GetException().Message})", Message.InfoType.Error);

        /// <summary>
        /// Record a Message object.
        /// </summary>
        public static void Log(Message message) => Messages.Add(message);

        /// <summary>
        /// Print out all logged Messages and remove them from memory.
        /// </summary>
        /// <param name="exit">Determines if the program will exit after displaying all messages.</param>
        public static void Print(bool exit = false)
        {
            Print(Messages);
            if (exit) Environment.Exit(0);
        }

        /// <summary>
        /// Defines the supported message types.
        /// </summary>
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

            if (message.Type == Message.InfoType.Error) StringService.Colorize("Error: ", ConsoleColor.DarkRed);
            else if (message.Type == Message.InfoType.Warning) StringService.Colorize("Warning: ", ConsoleColor.DarkYellow);

            Console.WriteLine(message.Text);
            Messages.Remove(message);
        }
    }
}