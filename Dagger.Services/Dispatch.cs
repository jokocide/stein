using System.Collections.Generic;
using Dagger.Data.Models;
using Dagger.Services.Pipelines;
using Dagger.Services.Routines;

namespace Dagger.Services
{
    /// <summary>
    /// Examine the received arguments and respond with some Routine-typed object.
    /// </summary>
    /// <returns>
    /// A class instance that derives from the Routine abstract class.
    /// </returns>
    public static class Dispatch
    {
        private static int MaxTotalArgs { get; } = 3;

        public static Routine Evaluate(string[] arguments)
        {
            List<string> args = new List<string>(arguments);
            
            // We can return right away if too many arguments are passed in.
            if (args.Count > MaxTotalArgs)
                return new Help(new Message
                {
                    message = "Too many arguments were received.",
                    type = Message.Type.Error
                });

            // foreach is not allowed when assigning to iterator.
            for (int count = 0; count < args.Count; count++) args[count] = args[count].ToLower();

            switch (args[0])
            {
                case "help":
                    return new HelpPipeline(args).Execute();
                case "build":
                    return new BuildPipeline(args).Execute();
                case "new":
                    return new NewPipeline(args).Execute();
                case "serve":
                    return new ServePipeline(args).Execute();
                default:
                    return new NotRecognized();
            }
        }
    }
}