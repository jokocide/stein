using System.IO;

namespace Stein.Services
{
    public static class PathService
    {
        public static string ResourcesPath => Path.Join(Directory.GetCurrentDirectory(), "resources");

        public static string SitePath => Path.Join(Directory.GetCurrentDirectory(), "site");

        public static string PagesPath => Path.Join(ResourcesPath, "pages");

        public static string[] PagesFiles => Directory.GetFiles(PagesPath);

        public static string TemplatesPath => Path.Join(ResourcesPath, "templates");

        public static string[] TemplatesFiles => Directory.GetFiles(TemplatesPath);

        public static string CollectionsPath => Path.Join(ResourcesPath, "collections");

        public static string[] CollectionsDirectories => Directory.GetDirectories(CollectionsPath);

        public static string PartialsPath => Path.Join(TemplatesPath, "partials");

        public static string[] PartialsFiles => Directory.GetFiles(PartialsPath);

        public static string ResourcesPublicPath => Path.Join(ResourcesPath, "public");

        public static string SitePublicPath => Path.Join(SitePath, "public");

        public static void Synchronize(string source, string destination, bool recursive = false)
        {
            DirectoryInfo dir = new DirectoryInfo(source);

            if (!dir.Exists) return;

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destination);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destination, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (!recursive) return;

            foreach (DirectoryInfo subDirectory in dirs)
            {
                string tempPath = Path.Combine(destination, subDirectory.Name);
                Synchronize(subDirectory.FullName, tempPath, true);
            }
        }

        public static bool IsProject(string path = null)
        {
            path ??= Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, "stein.json"));
        }

        public static string GetOutputPath(FileInfo file)
        {
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(file.Name);

            if (file.Directory.Name == "pages" && fileNameNoExtension == "index")
            {
                return Path.Join(PathService.SitePath, "index.html");
            }

            else if (file.Directory.Name == "pages")
            {
                string newDirectory = Path.Join(PathService.SitePath, fileNameNoExtension);
                Directory.CreateDirectory(newDirectory);
                return Path.Join(newDirectory, "index.html");
            }

            else
            {
                string newDirectory = Path.Join(PathService.SitePath, file.Directory.Name, fileNameNoExtension);
                Directory.CreateDirectory(newDirectory);
                return Path.Join(newDirectory, "index.html");
            }
        }

        public static string GetIterablePath(FileInfo file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            return $"/{file.Directory.Name}/{fileName}/";
        }

        public static string ReadAllSafe(string path)
        {
            string text;

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new StreamReader(stream);
                text = reader.ReadToEnd();
            }

            return text;
        }
    }
}