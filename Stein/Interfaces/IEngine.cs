using System.Collections.Generic;
using Stein.Models;

namespace Stein.Interfaces
{
    public interface IEngine
    {
        public void RegisterPartial(IEnumerable<string> files);

        public void RegisterPartial(string name, string body);

        public IEnumerable<IRenderer> CompileTemplate(IEnumerable<string> files);

        public IRenderer CompileTemplate(string body);

        public string RenderTemplate(IRenderer template, Injectable injectable = null);
    }
}