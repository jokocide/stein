using Stein.Interfaces;
using Stein.Pipelines;

namespace Stein.Models
{
    public abstract class Pipeline
    {
        public Pipeline(string[] args) => Args = args;

        public static IEvaluator GetPipeline(string[] args)
        {
            if (args.Length == 0 || args[0].ToLower() == "help") 
                return new HelpPipeline(args);

            if (args[0].ToLower() == "build") 
                return new BuildPipeline(args);

            if (args[0].ToLower() == "new") 
                return new NewPipeline(args);

            if (args[0].ToLower() == "serve") 
                return new ServePipeline(args);

            return new NotRecognizedPipeline(args);
         }

        protected string[] Args { get; }
    }
}