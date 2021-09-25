using System;
using System.IO;
using Stein.Interfaces;
using Stein.Models;

namespace Stein.Templates
{
    public class HandlebarsTemplate : Template, IRenderer
    {
        public HandlebarsTemplate(FileInfo fileInfo, object templateObject) : base(fileInfo, templateObject) { }

        public Writable Render() 
        {
            throw new NotImplementedException();
        }
    }
}