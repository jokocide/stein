using System;
using Dagger.Abstract;

namespace Dagger.Routines
{
    public class Build : Routine
    {
        /// <summary>
        /// Look for a resources folder in the current directory, or verify we are currently 
        /// inside of a resources folder and automatically compile all Markdown and HTML documents
        /// that are found in the expected locations.
        /// </summary>
        public override void Execute()
        {
            Console.WriteLine("Build routine.");
        }
    }
}