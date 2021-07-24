using System;
using System.Collections.Generic;
using System.IO;

using Dagger.Data.Models;

namespace Dagger.Services
{
    /// <summary>
    /// Methods for commiting Writable objects to disk.
    /// </summary>
    public class Author
    {
        public List<Writable> Pending { get; }

        public Author(List<Writable> pending)
        {
            Pending = pending;
        }
        
        public void Write()
        {
            foreach (Writable obj in Pending)
            {
                string path = Path.GetDirectoryName(obj.SitePath);
                if (path != null) Directory.CreateDirectory(path);
                File.WriteAllText(obj.SitePath, obj.Body);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"({DateTime.Now.ToString("t")}) ");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(obj.SitePath);
                Console.ResetColor();
            }

        }
    }
}