using System.Collections.Generic;
using Dagger.Data.Models;

namespace Dagger.Services
{
    /// <summary>
    /// Keep track of certain object types as they are created and disposed during a Build Routine.
    /// </summary>
    public class Store
    {
        public Dictionary<string, List<Dictionary<string, string>>> Collections { get; } = new();
        public List<Writable> Writable { get; } = new();
    }
}