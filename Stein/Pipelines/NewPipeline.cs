using System.IO;
using Stein.Interfaces;
using Stein.Models;
using Stein.Routines;
using Stein.Services;

namespace Stein.Pipelines
{
    public sealed class NewPipeline : Pipeline, IEvaluator
    {
        public NewPipeline(string[] args) : base(args) { }

        public IExecutable Evaluate()
        {
            if (Args.Length > MaxNewArgs)
            {
                MessageService.Log(Message.TooManyArgs());
                MessageService.Print(true);
            }

            return Args.Length > 1 ? PipelineNewPath(Args) : NewRoutine.GetDefault;
        }

        private int MaxNewArgs => 2;

        private IExecutable PipelineNewPath(string[] arguments)
        {
            if (!Directory.Exists(arguments[1]))
            {
                try
                {
                    Directory.CreateDirectory(arguments[1]);
                }
                catch (IOException)
                {
                    MessageService.Log(Message.ProvidedPathIsInvalid());
                    MessageService.Print(true);
                }
            }

            Directory.SetCurrentDirectory(arguments[1]);
            return NewRoutine.GetDefault;
        }
    }
}