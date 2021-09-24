using System.IO;

namespace Stein.Models
{
    public class Message
    {
        public static Message TooManyArgs()
        {
            Message message = new("Received too many arguments.", InfoType.Error);
            return message;
        }

        public static Message NoEngine()
        {
            Message message = new("Engine not specified in stein.json", InfoType.Error);
            return message;
        }

        public static Message NoTemplateKey(FileInfo fileInfo)
        {
            Message message = new($"No template key: {fileInfo.Name}", InfoType.Warning);
            return message;
        }

        public static Message TemplateNotFound(FileInfo fileInfo)
        {
            Message message = new($"Template not found: {fileInfo.Name}", InfoType.Error);
            return message;
        }

        public static Message ProvidedPathIsNotProject()
        {
            string text = "The provided path does not appear to be a Stein project. (Missing a stein.json file?)";
            Message message = new(text, Message.InfoType.Error);
            return message;
        }

        public static Message NotInProject(bool routineAcceptsPaths)
        {
            string text = routineAcceptsPaths
                ? $"Command was not called from a valid Stein project directory, and no path was provided."
                : $"Command was not called from a valid Stein project directory.";

            Message message = new(text, Message.InfoType.Error);
            return message;
        }

        public static Message CommandNotRecognized()
        {
            Message message = new("Command not recognized.", InfoType.Error);
            return message;
        }

        public static Message ProvidedPathIsInvalid()
        {
            Message message = new("The provided path does not appear to be valid.", InfoType.Error);
            return message;
        }

        public static Message NoExtension(FileInfo fileInfo)
        {
            Message message = new($"File has no extension: {fileInfo.Name}", InfoType.Error);
            return message;
        }

        public static Message InvalidJson(FileInfo fileInfo)
        {
            Message message = new($"Invalid JSON: {fileInfo.Name}", InfoType.Error);
            return message;
        }

        public Message(string text, InfoType type)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; init; }

        public InfoType Type { get; init; }

        public enum InfoType
        {
            Warning,
            Error
        }
    }
}