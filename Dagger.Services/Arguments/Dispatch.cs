using Dagger.Services.Pipelines;
using Dagger.Services.Routines;

namespace Dagger.Services.Arguments
{
    /// <summary>
    /// Hands the received arguments off to a Pipeline-typed object, which will ultimately return some kind of Routine
    /// that can be executed in Program.cs.
    /// </summary>
    /// <returns>
    /// Returns a Routine-typed object.
    /// </returns>
    public static class Dispatch
    {
        private static int MaxTotalArgs { get; } = 3;

        public static Routine Evaluate(string[] args)
        {
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
                case "watch":
                    return new WatchPipeline(args).Execute();
                default:
                    return new NotRecognized();
            }
        }
    }
}