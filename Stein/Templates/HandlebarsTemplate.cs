using System.IO;
using Stein.Models;

namespace Stein.Templates
{
    /// <summary>
    /// Represents a type of Template suitable for the Handlebars templating engine.
    /// </summary>
    public class HandlebarsTemplate : Template
    {
        /// <summary>
        /// Initializes a new instance of HandlebarsTemplate with the given FileInfo and an object
        /// derived from the Handlebars engine. 
        /// </summary>
        /// <param name="fileInfo">A FileInfo representation of the file.</param>
        /// <param name="templateObject">An object returned by HandlebarsEngine.</param>
        /// <returns></returns>
        /// <remarks>
        /// Handlebars.NET is a static library that returns an object to represent a compiled template.
        /// The templateObject provides a place to store that object within a new instance of HandlebarsTemplate.
        /// </remarks>
        public HandlebarsTemplate(FileInfo fileInfo, object templateObject) : base(fileInfo, templateObject) { }
    }
}