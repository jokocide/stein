using System;
using Dagger.Abstract;

namespace Dagger.Routines
{
    public class New : Routine
    {
        /// <summary>
        /// Create a new Dagger project.
        /// </summary>
        public override void Execute()
        {
            Console.WriteLine("New routine.");
        }
    }
}