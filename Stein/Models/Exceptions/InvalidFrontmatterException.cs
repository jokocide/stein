using System;

namespace Stein.Models.Exceptions
{
    class InvalidFrontmatterException : Exception
    {
        public override string Message { get; }
    }
}
