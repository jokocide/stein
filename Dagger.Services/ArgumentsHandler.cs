using System;
using System.IO;
using Dagger.Abstract;
using Dagger.Data.Models;
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
        private int _argsMaximum { get; } = 2;

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
                    if (_args.Length > 1)
                    {
                        // Expect next argument to be a path that leads to a Dagger project.
                        try
                        {
                            Directory.SetCurrentDirectory(_args[1]);
                        }
                        catch (IOException)
                        {
                            return new Help(new Message {message = $"'{_args[1]}' is not a valid path.", type = Message.Type.Error});
                        }
                    }

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