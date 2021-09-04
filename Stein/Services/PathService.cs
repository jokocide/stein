using System;
using System.IO;
using Stein.Models;

namespace Stein.Services
{
    /// <summary>
    /// Track important paths and provide methods to interact with and change those paths.
    /// </summary>
    public static class PathService
    {
        public static string ResourcesPath => Path.Join(Directory.GetCurrentDirectory(), "resources");
        public static string SitePath => Path.Join(Directory.GetCurrentDirectory(), "site");
        public static string PagesPath => Path.Join(ResourcesPath, "pages");
        public static string TemplatesPath => Path.Join(ResourcesPath, "templates");
        public static string CollectionsPath => Path.Join(ResourcesPath, "collections");
        public static string PartialsPath => Path.Join(TemplatesPath, "partials");
        public static string ResourcesPublicPath => Path.Join(ResourcesPath, "public");
        public static string SitePublicPath => Path.Join(SitePath, "public");

        /// <summary>
        /// Copy one directory to another, recursion is optional.
        /// </summary>
        /// <param name="source">
        /// A path representing the directory to be copied.
        /// </param>
        /// <param name="destination">
        /// A path representing the desired location of the copied files from sourceDirName.
        /// </param>
        /// <param name="recursive">
        /// A boolean to control recursive behavior. All subdirectories within sourceDirName
        /// will be copied recursively when true.
        /// </param>
        public static void Synchronize(string source, string destination, bool recursive = false)
        {
            DirectoryInfo dir = new DirectoryInfo(source);

            if (!dir.Exists)
            {
                string text = $"Unable to synchronize public files, no directory '{dir.FullName}' exists.";
                Message message = new(text, Message.InfoType.Error);
                MessageService.Log(message);
                return;
            }

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

        /// <summary>
        /// Return true if the given path is a Stein project, defaults to the current
        /// directory if no path is given.
        /// </summary>
        /// <param name="path">
        /// A path to the directory that will be tested.
        /// </param>
        /// <returns>
        /// Return True if the path is a Stein project.
        /// </returns>
        public static bool IsProject(string path = null)
        {
            path ??= Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, "stein.json"));
        }

        /// <summary>
        /// Return a path representing a suitable output location for the given file.
        /// </summary>
        /// <param name="file">
        /// A FileInfo object derived from the path of the file.
        /// </param>
        /// <returns>
        /// A string that represents a suitable output location.
        /// </returns>
        public static string GetOutputPath(FileInfo file)
        {
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(file.Name);

            return file.Directory?.Name switch
            {
                "pages" when fileNameNoExtension == "index" => IndexPagePath(),
                "pages" => NonIndexPagePath(fileNameNoExtension),
                _ => CollectionPath(fileNameNoExtension, file.Directory.Name)
            };
        }

        /// <summary>
        /// Return a path suitable for iterating over collection items in a template.
        /// </summary>
        /// <param name="file">
        /// A FileInfo object derived from the path of the file.
        /// </param>
        /// <returns>
        /// A string the represents a relative path from the project's site directory to the
        /// output location of this file.
        /// </returns>
        public static string GetIterablePath(FileInfo file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            return $"/{file.Directory.Name}/{fileName}/";
        }

        /// <summary>
        /// Return a path suitable for an index page file.
        /// </summary>
        /// <returns>
        /// A string that represents a suitable output location.
        /// </returns>
        private static string IndexPagePath() => Path.Join(PathService.SitePath, "index.html");

        /// <summary>
        /// Return a path suitable for non-index page files.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file.
        /// </param>
        /// <returns>
        /// A string that represents a suitable output location.
        /// </returns>
        private static string NonIndexPagePath(string fileName)
        {
            string newDirectory = Path.Join(PathService.SitePath, fileName);
            Directory.CreateDirectory(newDirectory);
            return Path.Join(newDirectory, "index.html");
        }

        /// <summary>
        /// Return a path suitable for collection files.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file.
        /// </param>
        /// <param name="directoryName">
        /// The name of the file's directory.
        /// </param>
        /// <returns>
        /// A string that represents a suitable output location.
        /// </returns>
        private static string CollectionPath(string fileName, string directoryName)
        {
            string newDirectory = Path.Join(PathService.SitePath, directoryName, fileName);
            Directory.CreateDirectory(newDirectory);
            return Path.Join(newDirectory, "index.html");
        }
    }
}