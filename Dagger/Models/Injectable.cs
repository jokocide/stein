using System.Collections.Generic;

namespace Dagger.Models
{
    /// <summary>
    /// An object that can be used to generate templates.
    /// </summary>
    public class Injectable
    {
        private Dictionary<string, List<Dictionary<string, string>>> Payload { get; }
    }
}