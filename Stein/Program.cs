using Stein.Models;

namespace Stein
{
     public class Program
    {
        private static void Main(string[] args)
        {
            new Parser(args)
                .Evaluate()?
                .Execute();

            Message.Print(true);
        }
    }
}
