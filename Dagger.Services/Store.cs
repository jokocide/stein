using System.Collections.Generic;
using Dagger.Data.Models;

namespace Dagger.Services
{
    /// <summary>
    /// Keep track of certain object types as they are created and disposed during a Build Routine.
    /// </summary>
    public static class Store
    {
        // Collections should not be implemented this way, but I'm just testing things out for now! :)
        public static Dictionary<string, List<Dictionary<string, string>>> Collections { get; } =
            new Dictionary<string, List<Dictionary<string, string>>>();
        
        public static List<Writable> Writable { get; } = new List<Writable>();
    }
}