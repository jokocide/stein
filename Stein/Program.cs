using Stein.Interfaces;
using Stein.Models;

namespace Stein
{
     public class Program
    {
        private static void Main(string[] args)
        {
            IEvaluator pipeline = Pipeline.GetPipeline(args);
            IExecutable routine = pipeline.Evaluate();
            routine.Execute();
        }
    }
}
