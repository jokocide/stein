using System.Collections.Generic;
using Dagger.Data.Models;
using Dagger.Services.Routines;

namespace Dagger.Services.Pipelines
{
    public class HelpPipeline : Pipeline
    {
        public HelpPipeline(List<string> args) : base(args) { }
        public override Routine Execute()
        {
            return Args.Count > 1
                ? new Help(new Message()
                {
                    message = $"Dagger doesn't support help specific to the command: {Args[1]}",
                    type = Message.Type.Warning
                })
                : new Help();
        }
    }
}