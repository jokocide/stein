using System.Collections.Generic;

namespace Dagger.Data.Models
{
    /// <summary>
    /// A data model that is used in the Build routine to keep track of collectable and writable files.
    /// </summary>
    public static class Store
    {
        // public static List<Dictionary<string, string>> Posts { get; } = new List<Dictionary<string, string>>(); todo: remove
        public static Dictionary<string, List<Dictionary<string, string>>> Collections { get; } =
            new Dictionary<string, List<Dictionary<string, string>>>();
        public static List<Writable> Writable { get; } = new List<Writable>();
    }
}