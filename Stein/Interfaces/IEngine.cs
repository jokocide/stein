using System.Collections.Generic;
using Stein.Models;

namespace Stein.Interfaces
{
    public interface IEngine
    {
        public void RegisterPartial(string path);

        public Template CompileTemplate(string path);

        public Writable RenderTemplate(Template template, Injectable injectable = null);
    }
}