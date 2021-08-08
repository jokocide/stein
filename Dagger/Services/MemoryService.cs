using System.Collections.Generic;
using Dagger.Models;

namespace Dagger.Services
{
    /// <summary>
    /// Keep track of objects as they are created through runtime and provide methods to use those items.
    /// </summary>
    public static class MemoryService
    {
        public static List<Collection> Collections { get; } = new();
        public static List<Writable> Writable { get; } = new();

        /// <summary>
        /// Clear all items from memory.
        /// </summary>
        public static void Clear()
        {
            Collections.Clear();
            Writable.Clear();
        }

        // Todo: Write code to generate an Injectable!
        // public Dictionary<string, List<Dictionary<string, string>>> GetInjectable()
        // {
        //     
        // }
    }
}