using System.Collections.Generic;

namespace Dagger.Data.Models
{
    public class Metadata
    {
        // The name of the collection that this item was derived from.
        public string Collection { get; init; }
        
        public string Body { get; }
        
        // An arbitrary amount of data that is pulled from a file's frontmatter.
        public Dictionary<string, string> Frontmatter { get; } = new Dictionary<string, string>();
    }
}