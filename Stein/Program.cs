using Stein.Models;

namespace Stein
{
     public class Program
    {
        private static void Main(string[] args)
        {
            Pipeline.GetPipeline(args).Evaluate().Execute();

            // Program.cs -- done
            // Models/Pipeline.cs -- done
            // Models/Routine.cs -- done
            // Pipelines/* -- done
            // Routines/* -- done
        }
    }
}
