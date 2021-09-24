using Stein.Interfaces;
using Stein.Models;
using HandlebarsDotNet;
using System.IO;
using Stein.Services;

namespace Stein.Engines
{
    public class HandlebarsEngine : Engine, IEngine
    {
        public void ClaimPartials(string directory)
        {
            string[] files = Directory.GetFiles(directory, "*.hbs");

            foreach (string path in files)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                string text = PathService.ReadAllSafe(path);

                Handlebars.RegisterTemplate(name, text);
            }
        }

        public IRenderer CompileTemplate(string body)
        {
            Template template = new();
        }
    }
}