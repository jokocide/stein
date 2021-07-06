using Dagger.Abstract;
using Dagger.Routines;

namespace Dagger.Services
{
    /// <summary>
    /// Receive arguments and determine which Routine to respond with.
    /// </summary>
    /// <returns>
    /// A class instance that derives from the Routine abstract class.
    /// </returns>
    public class ArgumentsHandler
    {
        private string[] _args { get; }

        // Represents the maximum amount of arguments.
        private int _argsMaximum { get; } = 1;

        public ArgumentsHandler(string[] args)
        {
            _args = args;
        }

        // Examine the received arguments and respond with a Routine instance.
        public Routine Evaluate() 
        {
            if (_args.Length <= _argsMaximum)
            {
                if (_args[0].ToLower() == "help")
                {
                    return new Help();
                }
                else if (_args[0].ToLower() == "build")
                {
                    return new Build();
                }
                else 
                {
                    return new NotRecognized();
                }
            }

            else return new NotRecognized();
        }
    }
}