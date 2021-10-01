using Stein.Interfaces;
using Stein.Templates;
using Stein.Models;
using HandlebarsDotNet;
using System.IO;
using Stein.Services;

namespace Stein.Engines
{
    public class HandlebarsEngine : Engine, IEngine
    {
        /// <summary>
        /// The Handlebars.NET library is static, so this is just an abstraction that passes the
        /// partial along.
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

            string text = PathService.ReadAllSafe(path);
            var templateObject = Handlebars.Compile(text);

            return new HandlebarsTemplate(new FileInfo(path), templateObject);
        }

        /// <summary>
        /// Render a Template object into a Writable.
        /// </summary>
        /// <param name="injectable">Optional data to be injected into the template.</param>
        public Writable RenderTemplate(Template template, Injectable injectable = null)
        {
            if (template is HandlebarsTemplate castedTemplate)
            {
                var castedObject = (HandlebarsTemplate<object, object>)castedTemplate.TemplateObject;
                string result = castedObject(injectable.Items);
                return new Writable(castedTemplate.Info, result);
            }

            return null;
        }
    }
}