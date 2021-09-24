using Stein.Interfaces;
using Stein.Models;
using HandlebarsDotNet;

namespace Stein.Engines
{
    public class HandlebarsEngine : IEngine
    {
        public void RegisterPartial(string name, string body)
        {
            Handlebars.RegisterTemplate(name, body);
        }

        public Template CompileTemplate(string body)
        {
            Template template = new();
        }
    }
}