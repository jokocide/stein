using System;
using Stein.Interfaces;
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
            foreach (string path in files)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                string body = PathService.ReadAllSafe(path);

                RegisterPartial(name, body);
            }
        }

        public void RegisterPartial(string name, string text) => Handlebars.RegisterTemplate(name, text);

        public IEnumerable<IRenderer> CompileTemplate(IEnumerable<string> paths)
        {
            throw new NotImplementedException();
        }

        public IRenderer CompileTemplate(string text)
        {
            throw new NotImplementedException();
        }

        public string RenderTemplate(IRenderer template, Injectable injectable = null)
        {
            throw new NotImplementedException();
        }
    }
}