using Dagger.Services.Pipelines;
using Dagger.Services.Routines;

namespace Dagger.Services.Arguments
{
    /// <summary>
    /// Pass the received arguments through a Pipeline to determine which Routine should be returned.
    /// </summary>
    /// <returns>
    /// A Routine object.
    /// </returns>
    public static class Dispatch
    {
        private static int MaxTotalArgs { get; } = 3;

        public static Routine Evaluate(string[] args)
        {
            // Default action will be to return a Help routine.
            if (args.Length == 0) return new HelpRoutine();
            
            // We can return right away if too many arguments are passed in.
            if (args.Length > MaxTotalArgs) return HelpRoutine.TooManyArguments();

            // Assert arguments are lowercase.
            for (int count = 0; count < args.Length; count++) args[count] = args[count].ToLower();

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
                    return HelpRoutine.CommandNotRecognized();
            }
        }
    }
}