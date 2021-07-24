using System.Collections.Generic;

using Dagger.Services.Pipelines;
using Dagger.Services.Routines;

namespace Dagger.Services
{
    /// <summary>
    /// Capable of examining a string[] of arguments and calling on different pipelines to determine which
    /// routine we should execute.
    /// </summary>
    public static class Dispatch
    {
        private static int MaxTotalArgs { get; } = 3;

        public static Routine Evaluate(string[] arguments)
        {
            // todo: Better documentation. Why are the arguments placed in a List<string> instead of keeping them in string[]?
            List<string> args = new List<string>(arguments);
            
            // We can return right away if too many arguments are passed in.
            if (args.Count > MaxTotalArgs) 
                return Help.TooManyArguments();

            /*
             * Make sure all of the arguments are lowercase,
             * foreach is not allowed when assigning to iterator so we use a for loop here.
             */
            for (int count = 0; count < args.Count; count++) 
                args[count] = args[count].ToLower();

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
                case "watch":
                    return new WatchPipeline(args).Execute();
                default:
                    return new NotRecognized();
            }
        }
    }
}