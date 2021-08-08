using System.Collections.Generic;

namespace Dagger.Models
{
    /// <summary>
    /// Represents an assembly of Resources that have been transformed into a format suitable for template
    /// generation and sorted based on any existing Date property.
    /// </summary>
    public class Injectable
    {
        /// <summary>
        /// The data to be injected into the template.
        /// </summary>
        private Dictionary<string, List<Dictionary<string, string>>> Payload { get; }
    }
}