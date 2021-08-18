namespace Stein.Models
{
    /// <summary>
    /// Base class for all Routine types. A Routine is a set of instructions that are carried out in response
    /// to the received command line arguments.
    /// </summary>
    public abstract class Routine
    {
        public abstract void Execute();
    }
}