using Stein.Models;

namespace Stein.Interfaces
{
    /// <summary>
    /// Defines the responsibilities of new templating systems.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// If the templating engine supports the use of partials, provide a method to register
        /// them.
        /// </summary>
        public void RegisterPartial(string path);

        /// <summary>
        /// Provide a method to return some derivative of Template from a file at path.
        /// </summary>
        public Template CompileTemplate(string path);

        /// <summary>
        /// Render a compiled Template with the given Injectable.
        /// </summary>
        public string RenderTemplate(Template template, Injectable injectable = null);
        
        public string RenderTemplate(Template template, SerializedItem serializedItem = null);
    }
}