using Dagger.Pipelines;
using Dagger.Routines;

namespace Dagger.Services
{
    /// <summary>
    /// Pass the received arguments through a Pipeline to determine which Routine should be returned.
    /// </summary>
    /// <returns>
    /// A Routine object.
    /// </returns>
    public static class PipelineService
    {
        private static int MaxTotalArgs => 3;

        public static Routine Evaluate(string[] args)
        {
            // Default action will be to return a Help routine.
            if (args.Length == 0) return new HelpRoutine();
            
            // We can return right away if too many arguments are passed in.
            if (args.Length > MaxTotalArgs) return HelpRoutine.TooManyArguments();

            return args[0].ToLower() switch
            {
                "help" => new HelpPipeline(args).Execute(),
                "build" => new BuildPipeline(args).Execute(),
                "new" => new NewPipeline(args).Execute(),
                "serve" => new ServePipeline(args).Execute(),
                _ => HelpRoutine.CommandNotRecognized()
            };
        }
    }
}