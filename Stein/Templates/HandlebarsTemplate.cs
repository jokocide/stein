using System.IO;
using Stein.Models;

namespace Stein.Templates
{
    public class HandlebarsTemplate : Template
    {
        public HandlebarsTemplate(FileInfo fileInfo, object templateObject) : base(fileInfo, templateObject) { }
    }
}