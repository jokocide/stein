using Stein.Models;

namespace Stein.Routines
{
    public sealed class NotRecognizedRoutine : Routine
    {
        public override void Execute()
        {
            Message.Log(Message.CommandNotRecognized());
            return;
        }
    }
}
