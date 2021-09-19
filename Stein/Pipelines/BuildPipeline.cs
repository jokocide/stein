using System.IO;
using Stein.Interfaces;
using Stein.Models;
using Stein.Routines;
using Stein.Services;

namespace Stein.Pipelines
{
    public sealed class BuildPipeline : Pipeline, IEvaluator
    {
        private int MaxBuildArgs => 2;

        public BuildPipeline(string[] args) : base(args) { }

        public IExecutable Evaluate()
        {
            if (Args.Length > MaxBuildArgs)
            {
                MessageService.Log(Message.TooManyArgs());
                MessageService.Print(true);
            }

            if (Args.Length > 1) return PipelineBuildPath(Args);

            if (!PathService.IsProject())
            {
                MessageService.Log(Message.NotInProject(true));
                MessageService.Print(true);
            }

            return new BuildRoutine();
        }

        private static IExecutable PipelineBuildPath(string[] arguments)
        {
            try
            {
                Directory.SetCurrentDirectory(arguments[1]);
            }
            catch (IOException)
            {
                MessageService.Log(Message.ProvidedPathIsNotProject());
                MessageService.Print(true);
            }

            if (!PathService.IsProject())
            {
                MessageService.Log(Message.NotInProject(true));
                MessageService.Print(true);
            }

            return new BuildRoutine();
        }
    }
}