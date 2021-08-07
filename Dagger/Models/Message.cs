namespace Dagger.Models
{
    /// <summary>
    /// Represents a message that is passed to the Help Routine.
    /// </summary>
    public class Message
    {
        public string Text { get; init; }
        public InfoType Type { get; init; }

        public enum InfoType
        {
            Warning,
            Error
        }
    }
}