using System.Collections.Generic;
using Dagger.Models;

namespace Dagger.Storage
{
    /// <summary>
    /// A basic storage scheme that allows for storing key value pairs.
    /// </summary>
    public sealed class KeyValueStore : Store
    {
        internal Dictionary<string, string> Pairs { get; }
    }
}