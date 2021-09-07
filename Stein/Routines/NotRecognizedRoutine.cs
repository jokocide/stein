using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    class NotRecognizedRoutine : Routine
    {
        public override void Execute()
        {
            MessageService.Log(Message.CommandNotRecognized());
            MessageService.Print(true);
        }
    }
}
