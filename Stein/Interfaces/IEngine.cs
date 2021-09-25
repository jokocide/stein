using System.Collections.Generic;
using Stein.Models;

namespace Stein.Interfaces
{
    public interface IEngine
    {
        public void RegisterPartial(IEnumerable<string> paths);

        public void RegisterPartial(string path);

        public IEnumerable<IRenderer> CompileTemplate(IEnumerable<string> paths);

        public IRenderer CompileTemplate(string path);

        public IEnumerable<Writable> RenderTemplate(IEnumerable<IRenderer> templates, Injectable injectable = null);

        public Writable RenderTemplate(IRenderer template, Injectable injectable = null);
    }
}