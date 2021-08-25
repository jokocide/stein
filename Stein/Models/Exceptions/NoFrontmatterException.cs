using System;

namespace Stein.Models.Exceptions
{
    class NoFrontmatterException : Exception
    {
        public override string Message { get; }
    }
}
