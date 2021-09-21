using System.IO;
using Stein.Interfaces;
using Stein.Models;
using Stein.Routines;
using Stein.Services;

namespace Stein.Pipelines
{
    public sealed class ServePipeline : Pipeline, IEvaluator
    {
        public ServePipeline(string[] args) : base(args) { }

        public IExecutable Evaluate()
        {
            if (Args.Length > MaxServeArgs)
            {
                MessageService.Log(Message.TooManyArgs());
                MessageService.Print(true);
            }

            if (Args.Length > 1) return ServePathPipeline(Args);

            if (!PathService.IsProject())
            {
                MessageService.Log(Message.NotInProject(true));
                MessageService.Print(true);
            }

            return ServeRoutine.GetDefault;
        }

        private int MaxServeArgs => 3;

        private IExecutable ServePathPipeline(string[] args)
        {
            if (args.Length > 2) return ServePathPortPipeline(args);

            if (!PathService.IsProject(args[1]))
            {
                if (int.TryParse(args[1], out _) && (args[1].Length == 4 || args[1].Length == 5))
                {
                    return new ServeRoutine(new Configuration(), args[1]);
                }

                MessageService.Log(Message.ProvidedPathIsNotProject());
                MessageService.Print(true);
            }

            Directory.SetCurrentDirectory(args[1]);
            return ServeRoutine.GetDefault;
        }

        private IExecutable ServePathPortPipeline(string[] args)
        {
            if (!PathService.IsProject(args[1]))
            {
                MessageService.Log(Message.ProvidedPathIsNotProject());
                MessageService.Print(true);
            }

            Directory.SetCurrentDirectory(args[1]);
            return new ServeRoutine(new Configuration(), args[2]);
        }
    }
}