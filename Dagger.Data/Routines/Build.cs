using System;
using Dagger.Abstract;

namespace Dagger.Routines
{
    /// <summary>
    /// Verify we are inside of a Dagger project folder or are at least given the path of one,
    /// and attempt to compile all Markdown and HTML documents associated with the project.
    /// </summary>
    public class Build : Routine
    {
        public override void Execute()
        {
            Console.WriteLine("Build routine.");
        }
    }
}