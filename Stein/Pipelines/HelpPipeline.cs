using Stein.Models;
using Stein.Routines;
using Stein.Services;

namespace Stein.Pipelines
{
    /// <summary>
    /// A Pipeline to handle the help command.
    /// </summary>
    public sealed class HelpPipeline : Pipeline
    {
        public HelpPipeline(string[] arguments) : base(arguments) { }

        /// <summary>
        /// Return a HelpRoutine.
        /// </summary>
        /// <returns>
        /// A Routine object.
        /// </returns>
        public override Routine Execute()
        {
            return Arguments.Length > 1 ? PipelineHelpTopic() : new HelpRoutine();
        }

        private Routine PipelineHelpTopic()
        {
            string topic = Arguments[1].ToLower();

            if (topic != "build" && topic != "new" && topic != "serve")
            {
                MessageService.Log(Message.CommandNotRecognized());
                MessageService.Print(true);
            }

            return topic switch
            {
                "build" => new HelpRoutine(HelpRoutine.HelpTopic.Build),
                "new" => new HelpRoutine(HelpRoutine.HelpTopic.New),
                _ => new HelpRoutine(HelpRoutine.HelpTopic.Serve)
            };
        }
    }
}