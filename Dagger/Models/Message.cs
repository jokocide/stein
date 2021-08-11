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

        public enum InfoType
        {
            Warning,
            Error,
            Critical
        }
    }
}