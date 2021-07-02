using System;

namespace Dagger.Routines
{
    public class Automatic : Routine
    {
        // This routine will look for a resources folder in the current directory
        // and attempt to process the contents.
        public override void Execute()
        {
            Console.WriteLine("Automatic routine");
        }
    }
}