using Dagger.Abstract;
using Dagger.Routines;
using Dagger.Services;

namespace Dagger.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Routine instructions = new Build();
            
            /// <Middleware>
            /// Actions that need to run before the routine can go here.
            /// </Middleware>

            // If arguments are received, use ArgumentsHandler to determine which routine we need to run.
            if (args.Length > 0)
            {
                ArgumentsHandler argumentsHandler = new ArgumentsHandler(args);
                instructions = argumentsHandler.Evaluate();
            }

            instructions.Execute();
        }
    }
}
