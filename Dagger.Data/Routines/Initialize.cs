using System;
using Dagger.Abstract;

namespace Dagger.Routines
{
    public class Initialize : Routine
    {
        /// <summary>
        /// Initialize a new Dagger project.
        /// 
        /// Dagger projects consist of 'resources' and 'site' folders for the input/output respectively,
        /// and a hidden file called '.dagger' to identify the project in the future.
        /// </summary>
        public override void Execute()
        {
            Console.WriteLine("Initialize routine.");
        }
    }
}