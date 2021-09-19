using Stein.Interfaces;
using Stein.Models;
using Stein.Routines;

namespace Stein.Pipelines
{
    public sealed class NotRecognizedPipeline : Pipeline, IEvaluator
    {
        public NotRecognizedPipeline(string[] args) : base(args) { }

        public IExecutable Evaluate()
        {
            return new NotRecognizedRoutine();
        }
    }
}
