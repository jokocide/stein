using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace Stein.Services
{
    /// <summary>
    /// Helper methods for interacting with strings that represent paths to files 
    /// and directories.
    /// </summary>
    public static class PathService
    {
        /// <summary>
        /// A path that refers to the resources directory of a project.
        /// </summary>
        public static string GetResourcesPath() => Path.Join(Directory.GetCurrentDirectory(), "resources");

        /// <summary>
        /// A path that refers to the site directory of a project.
        /// </summary>
        public static string GetSitePath() => Path.Join(Directory.GetCurrentDirectory(), "site");

        /// <summary>
        /// A path that refers to the templates directory of a project.
        /// </summary>
        public static string GetTemplatesPath() => Path.Join(GetResourcesPath(), "templates");

        /// <summary>
        /// A path that refers to the partials directory of a project.
        /// </summary>
        public static string GetPartialsPath() => Path.Join(GetTemplatesPath(), "partials");

        /// <summary>
        /// A path that refers to the resources/static directory of a project.
        /// </summary>
        public static string GetResourcesStaticPath() => Path.Join(GetResourcesPath(), "static");

        /// <summary>
        /// A path that refers to the site/static directory of a project.
        /// </summary>
        public static string GetSiteStaticPath() => Path.Join(GetSitePath(), "static");

        /// <summary>
        /// Copy the contents of a directory to a new location.
        /// </summary>
        /// <param name="source">The directory to be copied.</param>
        /// <param name="destination">The desired location of new copy.</param>
        /// <param name="recursive">Controls recursive functionality.</param>
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

        /// <summary>
        /// Return the contents of a file at path using a filehandle that allows other processes to
        /// access the same file, attempt to read the file again in the event that the file is 
        /// inaccessible.
        /// </summary>
        /// <param name="time">
        /// Controls the time delay between read attempts.
        /// </param>
        /// <returns></returns>
        public static string ReadAllSafe(string path, int time = 100)
        {
            string text;

            try
            {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var reader = new StreamReader(stream);
                    text = reader.ReadToEnd();
                }
            }
            catch (IOException)
            {
                Thread.Sleep(time);
                return ReadAllSafe(path, (time + 10));
            }

            return text;
        }

        /// <summary>
        /// Determine if the file at path should be ignored by Stein.
        /// </summary>
        /// <returns>
        /// Returns true if the file matches any of the rules defined in the method body, or false otherwise.
        /// </returns>
        /// <remarks>
        /// Most often used to ignore 'draft' files that aren't ready to be published.
        /// </remarks>
        public static bool IsIgnored(string path)
        {
            string name = Path.GetFileName(path);

            if (name == "static" || name == "templates" || name == "partials")
                return true;

            if (name.StartsWith("_")) return true;

            return false;
        }

        /// <summary>
        /// Return two organized lists of the pages and collections available within a project.
        /// </summary>
        /// <param name="path">The directory to be searched.</param>
        /// <param name="pages">Contains the page files during recursion.</param>
        /// <param name="collections">Contains the collection directories during recursion.</param>
        /// <returns>
        /// A tuple where Item1 represents the pages and Item2 represents the collections.
        /// </returns>
        public static (List<string> pages, List<string> collections) GetPagesAndCollections(
            string path,
            List<string> pages = null,
            List<string> collections = null)
        {
            pages ??= new();
            collections ??= new();

            string[] filesDirs = Directory.GetFileSystemEntries(path);
            foreach (string item in filesDirs)
            {
                if (IsIgnored(item))
                    continue;

                if (File.Exists(item))
                    pages.Add(item);

                else if (Directory.Exists(item))
                {
                    collections.Add(item);
                    GetPagesAndCollections(item, pages, collections);
                }
            }

            return (pages, collections);
        }
    }
}