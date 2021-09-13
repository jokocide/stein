namespace Stein.Models
{
    /// <summary>
    /// Represents a set of optional behaviors that are read and used 
    /// by Stein during a Routine.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Determines whether Message objects with Type of Warning
        /// are recorded and subsequently displayed after a Build routine.
        /// </summary>
        public bool SilenceWarnings { get; set; } = false;
    }
}