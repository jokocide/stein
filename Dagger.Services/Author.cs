using System;
using System.Collections.Generic;
using System.IO;

using Dagger.Data.Models;

namespace Dagger.Services
{
    /// <summary>
    /// Commit Writable objects to disk.
    /// </summary>
    public class Author
    {
        private List<Writable> Pending { get; }

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
            }

        }
    }
}