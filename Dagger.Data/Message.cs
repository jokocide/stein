namespace Dagger.Data.Models
{
    /// <summary>
    /// Represents a message that is passed to the Help routine.
    /// </summary>
    public class Message
    {
        public string message { get; set; }
        public Type type { get; set; }

        public enum Type
        {
            Warning,
            Error
        }
    }
}