using System;
using System.IO;

namespace Dagger.Data.Models
{
    public class Writable
    {
        public string ResourcePath { get; }
        public string SitePath { get; }
        public string Body { get; }

        public Writable(string resourcePath, string body)
        {
            ResourcePath = resourcePath;
            Body = body;
            
            // SitePath is based on resourcePath.
            SitePath = GenerateSitePath(resourcePath);
        }

        public string GenerateSitePath(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path); // gpg
            string directoryName = Path.GetDirectoryName(path);
            string sitePath; // To be returned later!
            
            if (fileName != "index") // post object
            {
                Directory.CreateDirectory(Path.Join("site", directoryName));
                sitePath = Path.Join("site", directoryName, fileName, "index.html");
            }
            else if (fileName == "index") // page object
            {
                Directory.CreateDirectory(Path.Join("site", directoryName));
                sitePath = Path.Join("site", directoryName, "index.html");
            }
            else
            {
                throw new ArgumentException($"Received a bad path: {path}");
            }

            Console.WriteLine($"Returned path: {sitePath}");
            return MakeHTML(sitePath);
        }

        private string MakeHTML(string path)
        {
            return Path.ChangeExtension(path, ".html");
        }
    }
}