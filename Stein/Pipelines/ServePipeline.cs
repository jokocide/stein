using System;
using System.IO;
using Stein.Interfaces;
using Stein.Models;
using Stein.Routines;
using Stein.Services;

namespace Stein.Pipelines
{
    public sealed class ServePipeline : Pipeline, IEvaluator
    {
        private int MaxServeArgs => 3;

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

            return new ServeRoutine();
        }

        private IExecutable ServePathPipeline(string[] args)
        {
            if (args.Length > 2) return ServePathPortPipeline(args);

            if (!PathService.IsProject(args[1]))
            {
                int number;

                if (Int32.TryParse(args[1], out number))
                {
                    return new ServeRoutine(args[1]);
                }

                MessageService.Log(Message.ProvidedPathIsNotProject());
                MessageService.Print(true);
            }

            Directory.SetCurrentDirectory(args[1]);
            return new ServeRoutine();
        }

        private IExecutable ServePathPortPipeline(string[] args)
        {
            if (!PathService.IsProject(args[1]))
            {
                MessageService.Log(Message.ProvidedPathIsNotProject());
                MessageService.Print(true);
            }

            Directory.SetCurrentDirectory(args[1]);
            return new ServeRoutine(args[2]);
        }
    }
}