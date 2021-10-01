using Stein.Models;

namespace Stein
{
     public class Program
    {
        private static void Main(string[] args)
        {
            Routine routine = new Parser().Evaluate(args);

            if (routine != null) 
                routine.Execute();

            Message.Print(true);
        }
    }
}
