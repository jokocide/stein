namespace Stein.Models
{
    /// <summary>
    /// Abstract base class for members shared by all inheriting classes.
    /// </summary>
    public abstract class Routine
    {
        /// <summary>
        /// A facade method to coordinate the execution of the Routine.
        /// </summary>
        public abstract void Execute();
    }
}
