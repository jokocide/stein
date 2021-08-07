using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using Dagger.Services;

namespace Dagger.Models
{
    /// <summary>
    /// Represents a fully processed item that is ready to be written to the file system.
    /// </summary>
    public class Writable
    {
        public string Target { get; }
        public string Payload { get; }

        /// <summary>Create a new Writable object.</summary>
        /// <param name="file">A FileInfo object used to generate the Writable.</param>
        /// <param name="payload">The string content to be written.</param>
        public Writable(FileInfo file, string payload)
        {
            Payload = payload;
            Target = GetTarget(file);
        }

        /// <summary>Return a path representing a suitable output location for the given file.</summary>
        /// <param name="file">A string representing the current location of the file.</param>
        /// <returns>A string that represents a suitable output location.</returns>
        private string GetTarget(FileInfo file)
        {
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(file.Name);
            return file.Directory.Name switch
            {
                "pages" when fileNameNoExtension == "index" => Path.Join(PathService.SitePath, "index.html"),
                "pages" => MakeNonIndexPagePath(fileNameNoExtension),
                _ => MakeCollectionPath(fileNameNoExtension, file.Directory.Name)
            };
        }

        /// <summary>
        /// Return a path suitable for non-index page files.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A string that represents a suitable output location.</returns>
        private string MakeNonIndexPagePath(string fileName) 
        {
            string newDirectory = Path.Join(PathService.SitePath, fileName);
            Directory.CreateDirectory(newDirectory);
            return Path.Join(newDirectory, "index.html");
        }

        /// <summary>
        /// Return a path suitable for collection files.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="directoryName">The name of the file's directory.</param>
        /// <returns>A string that represents a suitable output location.</returns>
        private string MakeCollectionPath(string fileName, string directoryName)
        {
            string newDirectory = Path.Join(PathService.SitePath, directoryName, fileName);
            Directory.CreateDirectory(newDirectory);
            return Path.Join(newDirectory, "index.html");
        }
    }
}