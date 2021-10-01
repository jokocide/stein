using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    public sealed class NotRecognizedRoutine : Routine
    {
        public override void Execute()
        {
            Message.Log(Message.CommandNotRecognized());
            Message.Print(true);
        }
    }
}
