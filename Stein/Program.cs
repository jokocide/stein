using Stein.Services;

namespace Stein
{
     public class Program
    {
        private static void Main(string[] args) => SetupService.Evaluate(args).Execute();
    }
}
