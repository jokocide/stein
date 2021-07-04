using System;
using Dagger.Abstract;

namespace Dagger.Routines
{
    public class Help : Routine
    {
        /// <summary>
        /// Used to display some helpful informatino related to the CLI, optionally
        /// includes a message to indicate what triggered this routine.
        /// </summary>
        /// <returns>
        /// A string value with instructions for using Dagger.
        /// </returns>
        private string _message { get; }

        public Help(string message = null)
        {
            _message = message;
        }   
             
        public override void Execute()
        {
            // Write out some CLI information, include _message somewhere if it isn't null.
            if (_message != null)
            {
                Console.WriteLine(_message);
            }
            Console.WriteLine("Help goes here!");
        }
    }
}