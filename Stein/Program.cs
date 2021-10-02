using Stein.Models;
using Stein.Services;
using System.Collections.Generic;

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
