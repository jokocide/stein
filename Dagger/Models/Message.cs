namespace Dagger.Models
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
        /// Return a Help routine that displays text to explain that Dagger has received more than the maximum
        /// number of allowed arguments.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Message TooManyArguments()
        {
            Message message = new("Received too many arguments.", Message.InfoType.Error);
            return message;
        }
        
         /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received a path argument which does
        /// not lead to a Dagger project as indicated by the presence of a hidden .dagger file.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Message ProvidedPathIsNotProject()
        {
            string text = "The provided path does not appear to be a Dagger project. (Missing a .dagger file?)";
            Message message = new(text, Message.InfoType.Error);
            return message;
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received a command but cannot proceed
        /// because a path was not provided, and was also not called from the directory of a Dagger project.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Message NotInDaggerProject(bool routineAcceptsPaths)
        {
            string text = routineAcceptsPaths
                ? $"Command was not called from a valid Dagger project directory, and no path was provided."
                : $"Command was not called from a valid Dagger project directory.";

            Message message = new(text, Message.InfoType.Error);
            return message;
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger has received arguments but does not
        /// understand how to respond to them.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Message CommandNotRecognized()
        {
            Message message = new("Command not recognized.", Message.InfoType.Error);
            return message;
        }

        /// <summary>
        /// Return a Help routine that displays text to explain that Dagger received a path argument, but
        /// the path argument does not appear to be valid. Dagger cannot create or move into the directory.
        /// </summary>
        /// <returns>A Routine object.</returns>
        public static Message ProvidedPathIsInvalid()
        {
            Message message = new("The provided path does not appear to be valid.", Message.InfoType.Error);
            return message;
        }

        public enum InfoType
        {
            Warning,
            Error,
            Critical
        }
    }
}