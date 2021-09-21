using Stein.Interfaces;
using Stein.Models;
using Stein.Services;

namespace Stein.Routines
{
    public sealed class NotRecognizedRoutine : IExecutable
    {
        public void Execute()
        {
            MessageService.Log(Message.CommandNotRecognized());
            MessageService.Print(true);
        }
    }
}
