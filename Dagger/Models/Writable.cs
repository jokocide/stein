using System.IO;
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
            Target = PathService.GetOutputPath(file);
        }
    }
}