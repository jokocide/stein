using System.IO;
using Dagger.Data.Models;
using Dagger.Services.Routines;

namespace Dagger.Services
{
    /// <summary>
    /// Receive arguments and determine which Routine to respond with.
    /// </summary>
    /// <returns>
    /// A class instance that derives from the Routine abstract class.
    /// </returns>
    public class Dispatch
    {
        private string[] _args { get; }

        public Dispatch(string[] args)
        {
            _args = args;
        }

        // Examine the received arguments and respond with a Routine instance.
        public Routine Evaluate() 
        {
            if (_args[0].ToLower() == "help") // dagger help - display help info
            {
                return new Help();
            }
            else if (_args[0].ToLower() == "build") // dagger build - process project
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
                        return new Help(new Message { message = $"'{_args[1]}' is not a valid path.", type = Message.Type.Error });
                    }
                }

                if (Helper.CheckIsProject())
                {
                    return new Build();
                }
                else
                {
                    return new Help(new Message { message = "Provide a path to a Dagger project or move to project before calling build.", type = Message.Type.Error });
                }
            }
            else if (_args[0].ToLower() == "new") // dagger new - create new project
            {
                if (!Helper.CheckIsProject())
                {
                    return new New();
                }
                else
                {
                    return new Help(new Message { message = "A project has already been initialized in this directory.", type = Message.Type.Error });
                }
                // Create resources folder
                // Create site folder
                // Create site folder
            }
            else
            {
                return new Help(new Message { message = "Command not recognized.", type = Message.Type.Error });
            }
        }
    }
}