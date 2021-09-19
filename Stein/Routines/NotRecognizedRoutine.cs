using Stein.Interfaces;
using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    class NotRecognizedRoutine : IExecutable
    {
        public void Execute()
        {
            MessageService.Log(Message.CommandNotRecognized());
            MessageService.Print(true);
        }
    }
}
