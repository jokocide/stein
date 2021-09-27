using Stein.Interfaces;
using Stein.Templates;
using Stein.Models;
using HandlebarsDotNet;
using System.IO;
using Stein.Services;
using System.Collections.Generic;

namespace Stein.Engines
{
    public class HandlebarsEngine : Engine, IEngine
    {
        public void RegisterPartial(IEnumerable<string> paths)
        {
            foreach (string path in paths) RegisterPartial(path);
        }

        public void RegisterPartial(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            string body = PathService.ReadAllSafe(path);

            Handlebars.RegisterTemplate(name, body);
        }

        public IEnumerable<Template> CompileTemplate(IEnumerable<string> paths)
        {
            List<Template> templates = new();

            foreach (string path in paths)
            {
                Template template = CompileTemplate(path);
                templates.Add(template);
            }

            return templates;
        }

        public Template CompileTemplate(string path)
        {
            string ext = Path.GetExtension(path);
            if (ext != ".hbs") return null;

            string text = PathService.ReadAllSafe(path);
            var templateObject = Handlebars.Compile(text);

            return new HandlebarsTemplate(new FileInfo(path), templateObject);
        }

        public IEnumerable<Writable> RenderTemplate(IEnumerable<Template> templates, Injectable injectable = null)
        {
            List<Writable> writables = new();

            foreach (Template template in templates)
            {
                Writable writable = RenderTemplate(template, injectable);
                writables.Add(writable);
            }

            return writables;
        }

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