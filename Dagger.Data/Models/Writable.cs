using System;
using System.IO;

namespace Dagger.Data.Models
{
    public class Writable
    {
        private string ResourcePath { get; }
        public string SitePath { get; }
        public string Body { get; }

        public Writable(string resourcePath, string body)
        {
            ResourcePath = resourcePath;
            Body = body;
            SitePath = MakeSitePath(resourcePath);
        }

        private string MakeSitePath(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            string directoryName = Path.GetDirectoryName(path);
            string parentOfDirectoryName = Path.GetDirectoryName(directoryName);

            if (directoryName == "pages" && parentOfDirectoryName != "collections")
                switch (fileName)
                {
                    case "index":
                        return Path.Join(Directory.GetCurrentDirectory(), "site", fileName + ".html");
                    default:
                        return MakeNonIndexPagePath(fileName);
                }
            
            if (parentOfDirectoryName != "collections")
                throw new Exception($"Unable to generate writable path for: {path}");

            return MakeCollectionPath(fileName, directoryName);
        }

        private string MakeNonIndexPagePath(string fileName) 
        {
            string newDirectory = Path.Join(Directory.GetCurrentDirectory(), "site", fileName);
            Directory.CreateDirectory(newDirectory);
            return Path.Join(newDirectory, "index.html");
        }

        private string MakeCollectionPath(string fileName, string directoryName)
        {
            string newDirectory = Path.Join(Directory.GetCurrentDirectory(), "site", directoryName, fileName);
            Directory.CreateDirectory(newDirectory);
            return Path.Join(newDirectory, "index.html");
        }
    }
}