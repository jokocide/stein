using System;
using System.Collections.Generic;
using System.IO;
using Dagger.Data.Models;

namespace Dagger.Services
{
    /// <summary>
    /// Methods for writing objects to disk after processing by a routine.
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
                Console.WriteLine($"Writing to: {obj.SitePath}");
                File.WriteAllText(obj.SitePath, obj.Body);
            }
        }
    }
}