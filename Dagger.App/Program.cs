using System;
using System.IO;
using Dagger.Routines;
using Dagger.Services;

namespace Dagger.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Routine instructions;
            
            if (args.Length == 0)
            {
                instructions = new Automatic();
                instructions.Execute();
            }
        }
    }
}
