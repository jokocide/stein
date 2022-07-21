using Stein.Interfaces;
using Stein.Templates;
using Stein.Models;
using HandlebarsDotNet;
using System.IO;
using Stein.Services;

namespace Stein.Engines
{
    public class HandlebarsEngine : IEngine
    {
        /// <summary>
        /// Register the file at path with the static Handlebars class.
        /// </summary>
        public void RegisterPartial(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            string body = PathService.ReadAllSafe(path);

            Handlebars.RegisterTemplate(name, body);
        }

        /// <summary>
        /// Return a compiled template from the file that exists at path.
        /// </summary>
        public Template CompileTemplate(string path)
        {
            string ext = Path.GetExtension(path);
            if (ext != ".hbs") return null;

            HandlebarsTemplate<object, object> templateObject = null;

            string text = PathService.ReadAllSafe(path);
            try
            {
                templateObject = Handlebars.Compile(text);
            }
            catch
            {
                return null;
            }

            return new HandlebarsTemplate(new FileInfo(path), templateObject);
        }

        /// <summary>
        /// Render a Template object into a Writable.
        /// </summary>
        /// <param name="injectable">Data to be injected into the template.</param>
        public string RenderTemplate(Template template, Injectable injectable)
        {
            if (template is HandlebarsTemplate castedTemplate)
            {
                var castedObject = (HandlebarsTemplate<object, object>)castedTemplate.TemplateObject;
                return castedObject(injectable.Items);
            }

            return null;
        }

        public string RenderTemplate(Template template, SerializedItem serializedItem)
        {
            if (template is HandlebarsTemplate castedTemplate)
            {
                var castedObject = (HandlebarsTemplate<object, object>)castedTemplate.TemplateObject;
                return castedObject(serializedItem);
            }

            return null;
        }
    }
}