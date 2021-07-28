using System.Collections.Generic;

namespace Dagger.Data.Models
{
    /// <summary>
    /// A data model that is used in the Build routine to keep track of collectable and writable files.
    /// </summary>
    public static class Store
    {
        // Collections should not be implemented this way, but I'm just testing things out for now! :)
        public static Dictionary<string, List<Dictionary<string, string>>> Collections { get; } =
            new Dictionary<string, List<Dictionary<string, string>>>();
        
        public static List<Writable> Writable { get; } = new List<Writable>();
    }
}