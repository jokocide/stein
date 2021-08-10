using System;
using System.IO;

namespace Dagger.Models
{
    /// <summary>
    /// Base class for all Store types.
    /// </summary>
    public abstract class Store
    {
        private string _template;
        
        /// <summary>
        /// A string representing the output path for a file, injected to allow for generating links
        /// during iteration in a template.
        /// </summary>
        public string Link { get; set; }
        
        /// <summary>
        /// User-provided string to represent a date, used during sorting if it can be parsed into a
        /// DateTime object.
        /// </summary>
        public string Date { get; set; }
       
        /// <summary>
        /// If a Resource has a 
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Stores the name of a requested template file, not including the extension.
        /// </summary>
        public string Template
        {
            get => _template;
            set
            {
                if (Path.HasExtension(value))
                    throw new InvalidOperationException("Received path with extension.");

                _template = value;
            }
        }
    }
}