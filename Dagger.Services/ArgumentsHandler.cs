using System;
using Dagger.Abstract;
using Dagger.Routines;

namespace Dagger.Services
{
    /// <summary>
    /// Receive and handle command line arguments.
    /// </summary>
    /// <returns>
    /// A Routine-typed object.
    /// </returns>
    public class ArgumentsHandler
    {
        private string[] _args { get; }

        // Define some 'rules' here that can be used by private 'validator' methods.
        private int _argsMaximum { get; } = 1;

        public ArgumentsHandler(string[] args)
        {
            _args = args;
        }

        public Routine Evaluate() // Return a routine based on the arguments that were passed in.
        {
            if (_args.Length <= _argsMaximum)
            {
                if (_args[0].ToLower() == "help")
                {
                    return new Help();
                }

                if (_args[0].ToLower() == "build")
                {
                    return new Build();
                }
                
                else // Arguments were received, but we don't understand them.
                {
                    return new NotRecognized();
                }
            }

            else return new NotRecognized();
        }
    }
}