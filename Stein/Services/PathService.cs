using System.IO;
using System.Collections.Generic;
using Stein.Models;
using System;

namespace Stein.Services
{
    public static class PathService
    {
        static PathService()
        {
            FindFiles(PathService.ResourcesPath);
        }

        public static string ResourcesPath => Path.Join(Directory.GetCurrentDirectory(), "resources");

        public static string SitePath => Path.Join(Directory.GetCurrentDirectory(), "site");

        public static IEnumerable<string> PagesFiles { get; set; }

        public static IEnumerable<string> CollectionsDirectories { get; set; }

        public static string TemplatesPath => Path.Join(ResourcesPath, "templates");

        public static string[] TemplatesFiles => Directory.GetFiles(TemplatesPath);

        public static string PartialsPath => Path.Join(TemplatesPath, "partials");

        public static string[] PartialsFiles => Directory.GetFiles(PartialsPath);

        public static string ResourcesStaticPath => Path.Join(ResourcesPath, "static");

        public static string SiteStaticPath=> Path.Join(SitePath, "static");

        public static IEnumerable<string> TryGetCollections(string path, List<string> collections = null)
        {
            collections ??= new();
            string[] dirs = Directory.GetDirectories(path);
            foreach(string dir in dirs)
            {
                if (IsIgnored(dir)) continue;
                collections.Add(dir);
                TryGetCollections(dir, collections);
            }
            return collections;
        }

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

        public static bool IsIgnored(string path)
        {
            string name = Path.GetFileName(path);

            if (name == "static" || name == "templates" || name == "partials") 
                return true;

            if (name.StartsWith("_")) return true;

            return false;
        }

        public static bool IsProject(string path = null)
        {
            path ??= Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, "stein.json"));
        }

        public static string GetOutputPath(FileInfo file)
        {
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(file.Name);
            string relative = Path.GetRelativePath(PathService.ResourcesPath, file.FullName);
            Configuration config = new ConfigurationService().GetConfig();

            if (relative == $"index.{config.Engine}")
            {
                return Path.Join(PathService.SitePath, "index.html");
            }
            else
            {
                string name = Path.GetFileNameWithoutExtension(relative);
                string parent = Path.GetDirectoryName(relative);
                string directory = Path.Join(PathService.SitePath, parent, name);
                Directory.CreateDirectory(directory);
                return Path.Join(directory, "index.html");
            }
        }

        public static string GetIterablePath(FileInfo file)
        {
            string relative = Path.GetRelativePath(PathService.ResourcesPath, file.FullName);
            string noExtension = Path.ChangeExtension(relative, null);
            string forwardSlashes = noExtension.Replace("\\","/");
            return $"/{forwardSlashes}/";


        }

        private static string MirrorPathToSite(FileInfo file)
        {
            string relative = Path.GetRelativePath(PathService.ResourcesPath, file.FullName);
            string name = Path.GetFileNameWithoutExtension(relative);
            string parent = Path.GetDirectoryName(relative);
            string directory = Path.Join(PathService.SitePath, parent, name);
            return directory;
        }

        public static string ReadAllSafe(string path)
        {
            string text;

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new StreamReader(stream);
                text = reader.ReadToEnd();
            }

            return text;
        }

        private static void FindFiles(string path, List<string> collections = null, List<string> pages = null)
        {
            collections ??= new();
            pages ??= new();

            Configuration config = new ConfigurationService().GetConfig();

            string[] filesDirs = Directory.GetFileSystemEntries(path);
            foreach(string item in filesDirs)
            {
                if (IsIgnored(item)) continue;

                if (File.Exists(item) && Path.GetExtension(item) == $".{config.Engine}") 
                pages.Add(item);

                else if (Directory.Exists(item))
                {
                    collections.Add(item);
                    FindFiles(item, collections, pages);
                }
            }

             CollectionsDirectories = collections;
             PagesFiles = pages;
        }
    }
}