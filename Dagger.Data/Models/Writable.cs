using System;
using System.IO;

namespace Dagger.Data.Models
{
    public class Writable
    {
        public string SitePath { get; }
        public string Body { get; }

        public Writable(string resourcePath, string body)
        {
            Body = body;
            SitePath = MakeSitePath(resourcePath);
        }

        private string MakeSitePath(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            
            string fileName = Path.GetFileNameWithoutExtension(directoryInfo.Name);
            string directoryName = directoryInfo.Parent?.Name;

            // Todo: This switch track needs more error handling, and we probably shouldn't default to collection file.
            return directoryName switch
            {
                // Resolving an index page file.
                "pages" when fileName == "index" => Path.Join(Directory.GetCurrentDirectory(), "site", "index.html"),
                // Resolving a non-index page file.
                "pages" => MakeNonIndexPagePath(fileName),
                // Resolving a collection file. 
                _ => MakeCollectionPath(fileName, directoryName)
            };
            
        }

        /// <summary>
        /// Return a path suitable for writing a page file that isn't already an index file to the file system.
        /// </summary>
        /// <param name="fileName">The name of the file. Should not include a file extension.</param>
        /// <returns>
        /// A string path representing a suitable location for this file within a Dagger project's site directory.
        /// </returns>
        private string MakeNonIndexPagePath(string fileName) 
        {
            string newDirectory = Path.Join(Directory.GetCurrentDirectory(), "site", fileName);
            
            // Assert the directory exists.
            Directory.CreateDirectory(newDirectory);

            // Console.WriteLine(newDirectory, "index.html");
            return Path.Join(newDirectory, "index.html");
        }

        /// <summary>
        /// Return a path suitable for writing a collection file to the file system.
        /// </summary>
        /// <param name="fileName">The name of the file. Should not include a file extension.</param>
        /// <param name="directoryName">
        /// The name of the directory that the file resides in, or name of the collection that this file belongs to.
        /// </param>
        /// <returns>
        /// A string path representing a suitable location for this file within a Dagger project's site directory.
        /// </returns>
        private string MakeCollectionPath(string fileName, string directoryName) // collections, guides
        {
            // Console.WriteLine(Path.Join(Directory.GetCurrentDirectory(), "site", directoryName, fileName));
            string newDirectory = Path.Join(Directory.GetCurrentDirectory(), "site", directoryName, fileName);
            
            // Assert the directory exists.
            Directory.CreateDirectory(newDirectory);
            
            // Console.WriteLine(Path.Join(newDirectory, "index.html"));
            return Path.Join(newDirectory, "index.html");
        }
    }
}