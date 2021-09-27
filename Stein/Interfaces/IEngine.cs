using System.Collections.Generic;
using Stein.Models;

namespace Stein.Interfaces
{
    public interface IEngine
    {
        public void RegisterPartial(IEnumerable<string> paths);

        public void RegisterPartial(string path);

        public IEnumerable<Template> CompileTemplate(IEnumerable<string> paths);

        public Template CompileTemplate(string path);

        public IEnumerable<Writable> RenderTemplate(IEnumerable<Template> templates, Injectable injectable = null);

        public Writable RenderTemplate(Template template, Injectable injectable = null);
    }
}