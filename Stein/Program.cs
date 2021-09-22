using Stein.Models;

namespace Stein
{
     public class Program
    {
        private static void Main(string[] args)
        {
            Pipeline.GetPipeline(args).Evaluate().Execute();
        }
    }
}
