using System.IO;

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

        public static Message NoEngine() => new("Engine not specified in stein.json", InfoType.Error);

        public static Message NoTemplateKey(FileInfo fileInfo) => new($"No template key: {fileInfo.Name}", InfoType.Warning);

        public static Message TemplateNotFound(FileInfo fileInfo) => new("Template not found: {fileInfo.Name}", InfoType.Error);

        public static Message ProvidedPathIsNotProject() => new("The provided path does not appear to be a Stein project. (Missing a stein.json file?)", InfoType.Error);

        public static Message NotInProject(bool routineAcceptsPaths)
        {
            string text = routineAcceptsPaths
                ? $"Command was not called from a valid Stein project directory, and no path was provided."
                : $"Command was not called from a valid Stein project directory.";

            Message message = new(text, Message.InfoType.Error);
            return message;
        }

        public static Message CommandNotRecognized() => new("Command not recognized.", InfoType.Error);

        public static Message ProvidedPathIsInvalid() => new("The provided path does not appear to be valid.", InfoType.Error);

        public static Message NoExtension(FileInfo fileInfo) => new($"File has no extension: {fileInfo.Name}", InfoType.Error);

        public static Message InvalidJson(FileInfo fileInfo) => new($"Invalid JSON: {fileInfo.Name}", InfoType.Error);

        public enum InfoType
        {
            Warning,
            Error
        }
    }
}