using System;
using Dagger.Abstract;
using Dagger.Services;

namespace Dagger.Routines
{
    /// <summary>
    /// Attempt to compile the Dagger project that exists in the current directory.
    /// </summary>
    public class Build : Routine
    {
        public override void Execute()
        {
            Console.WriteLine("Build routine.");
            Console.Write($"Result of check: {Helper.CheckIsProject()}");
        }
    }
}