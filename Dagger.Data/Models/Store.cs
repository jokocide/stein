using System.Collections.Generic;

namespace Dagger.Data.Models
{
    /// <summary>
    /// A data model that is used in the Build routine to keep track of collectable and writable files.
    /// </summary>
    public class Store
    {
        public List<Dictionary<string, string>> Posts { get; } = new List<Dictionary<string, string>>();
        public List<Writable> Writable { get; } = new List<Writable>();
    }
}