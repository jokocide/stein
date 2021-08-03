namespace Dagger.Data.Models
{
    /// <summary>
    /// Represents a message that is passed to the Help Routine.
    /// </summary>
    public class Message
    {
        public string Text { get; init; }
        public MessageType Type { get; init; }

        public enum MessageType
        {
            Warning,
            Error
        }
    }
}