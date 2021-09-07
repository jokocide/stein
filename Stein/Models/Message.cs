using System.IO;

namespace Stein.Models
{
    /// <summary>
    /// Represents a message generated during a Routine.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// A string message that can be displayed in stdout.
        /// </summary>
        public string Text { get; init; }
        
        /// <summary>
        /// A descriptor of the severity of the message.
        /// </summary>
        public InfoType Type { get; init; }

        public Message(string text, InfoType type)
        {
            Text = text;
            Type = type;
        }
        
        /// <summary>
        /// Return a Message to indicate that Stein has received more than the maximum number
        /// of allowed arguments.
        /// </summary>
        /// <returns>
        /// A Routine object.
        /// </returns>
        public static Message TooManyArguments()
        {
            Message message = new("Received too many arguments.", InfoType.Error);
            return message;
        }

        /// <summary>
        /// Return a Message to indicate that a file does not appear to have specified a Template key.
        /// </summary>
        /// <param name="fileInfo">A FileInfo object derived from the file.</param>
        /// <returns>A Message object.</returns>
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
        
        /// <summary>
        /// Return a Message to indicate that Stein has received a path argument which does
        /// not lead to a Stein project, as indicated by the presence of a stein.json file.
        /// </summary>
        /// <returns>
        /// A Routine object.
        /// </returns>
        public static Message ProvidedPathIsNotProject()
        {
            string text = "The provided path does not appear to be a Stein project. (Missing a stein.json file?)";
            Message message = new(text, Message.InfoType.Error);
            return message;
        }


        /// <summary>
        /// Return a Message to indicate that Stein has received a command but cannot proceed
        /// because a path was not provided, and was also not called from the directory of a Stein project.
        /// </summary>
        /// <returns>
        /// A Routine object.
        /// </returns>
        public static Message NotInProject(bool routineAcceptsPaths)
        {
            string text = routineAcceptsPaths
                ? $"Command was not called from a valid Stein project directory, and no path was provided."
                : $"Command was not called from a valid Stein project directory.";

            Message message = new(text, Message.InfoType.Error);
            return message;
        }

        /// <summary>
        /// Return a Message to indicate that Stein has received arguments but does not
        /// understand how to respond to them.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Message CommandNotRecognized()
        {
            Message message = new("Command not recognized.", InfoType.Error);
            return message;
        }

        /// <summary>
        /// Return a Message to indicate that Stein has received a path argument, but
        /// the path argument does not appear to be valid. Stein cannot create or move into the directory.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Message ProvidedPathIsInvalid()
        {
            Message message = new("The provided path does not appear to be valid.", InfoType.Error);
            return message;
        }

        public enum InfoType
        {
            Warning,
            Error
        }
    }
}