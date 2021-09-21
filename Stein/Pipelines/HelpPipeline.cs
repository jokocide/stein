using Stein.Interfaces;
using Stein.Models;
using Stein.Routines;
using Stein.Services;

namespace Stein.Pipelines
{
    public sealed class HelpPipeline : Pipeline, IEvaluator
    {
        public HelpPipeline(string[] args) : base(args) { }

        public IExecutable Evaluate()
        {
            if (Args.Length > MaxHelpArgs)
            {
                MessageService.Log(Message.TooManyArgs());
                MessageService.Print(true);
            }

            return Args.Length > 1 ? PipelineHelpTopic() : HelpRoutine.GetDefault;
        }

        private int MaxHelpArgs => 2;

        private IExecutable PipelineHelpTopic()
        {
            string topic = Args[1].ToLower();

            if (topic != "build" && topic != "new" && topic != "serve")
            {
                MessageService.Log(Message.CommandNotRecognized());
                MessageService.Print(true);
            }

            return topic switch
            {
                "build" => HelpRoutine.GetBuildTopic,
                "new" => HelpRoutine.GetNewTopic,
                _ => HelpRoutine.GetServeTopic
            };
        }
    }
}