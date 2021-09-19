using Stein.Interfaces;
using Stein.Pipelines;

namespace Stein.Models
{
    public abstract class Pipeline
    {
        protected string[] Args { get; }

        protected Pipeline(string[] args) => Args = args;
        
        public static IEvaluator GetPipeline(string[] args)
        {
            if (args.Length == 0 || args[0].ToLower() == "help") return new HelpPipeline(args);
            else if (args[0].ToLower() == "build") return new BuildPipeline(args);
            else if (args[0].ToLower() == "new") return new NewPipeline(args);
            else if (args[0].ToLower() == "serve") return new ServePipeline(args);
            else return new NotRecognizedPipeline(args);
        }
    }
}