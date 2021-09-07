using System.IO;

namespace Stein.Services
{
    /// <summary>
    /// Track important paths and provide methods to interact with and change those paths.
    /// </summary>
    public static class PathService
    {
        /// <summary>
        /// Refer to the Resources directory of a project within the current directory.
        /// </summary>
        public static string ResourcesPath => Path.Join(Directory.GetCurrentDirectory(), "resources");

        /// <summary>
        /// Refer to the Site directory of a project within the current directory.
        /// </summary>
        public static string SitePath => Path.Join(Directory.GetCurrentDirectory(), "site");

        /// <summary>
        /// Refer to the Pages directory of a project within the current directory.
        /// </summary>
        public static string PagesPath => Path.Join(ResourcesPath, "pages");

        /// <summary>
        /// Refer to the Templates directory of a project within the current directory.
        /// </summary>
        public static string TemplatesPath => Path.Join(ResourcesPath, "templates");

        /// <summary>
        /// Refer to the Collections directory of a project within the current directory.
        /// </summary>
        public static string CollectionsPath => Path.Join(ResourcesPath, "collections");

        /// <summary>
        /// Refer to the Templates/Partials directory of a project within the current directory.
        /// </summary>
        public static string PartialsPath => Path.Join(TemplatesPath, "partials");

        /// <summary>
        /// Refer to the Resources/Public directory of a project within the current directory.
        /// </summary>
        public static string ResourcesPublicPath => Path.Join(ResourcesPath, "public");

        /// <summary>
        /// Refer to the Site/Public directory of a project within the current directory.
        /// </summary>
        public static string SitePublicPath => Path.Join(SitePath, "public");

        /// <summary>
        /// Copy one directory to another with optional recursion.
        /// </summary>
        /// <param name="source">A path representing the directory to be copied.</param>
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
        /// Return true if the given path is a Stein project, defaults to the current
        /// directory if no path is given.
        /// </summary>
        /// <param name="path">A path to the directory that will be tested.</param>
        /// <returns>
        /// Return True if the path is a Stein project, which is indicated by the presence of
        /// a stein.json file in the path.
        /// </returns>
        public static bool IsProject(string path = null)
        {
            path ??= Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, "stein.json"));
        }

        /// <summary>
        /// Return a path representing a suitable output location for the given file.
        /// </summary>
        /// <param name="file">A FileInfo object derived from the path of the file.</param>
        /// <returns>A string that represents a suitable output location.</returns>
        public static string GetOutputPath(FileInfo file)
        {
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(file.Name);

            // Return for an 'index' page path.
            if (file.Directory.Name == "pages" && fileNameNoExtension == "index")
            {
                return Path.Join(PathService.SitePath, "index.html");
            }

            // Return for a page path.
            else if (file.Directory.Name == "pages")
            {
                string newDirectory = Path.Join(PathService.SitePath, fileNameNoExtension);
                Directory.CreateDirectory(newDirectory);
                return Path.Join(newDirectory, "index.html");
            }

            // Return for a collection item.
            else
            {
                string newDirectory = Path.Join(PathService.SitePath, file.Directory.Name, fileNameNoExtension);
                Directory.CreateDirectory(newDirectory);
                return Path.Join(newDirectory, "index.html");
            }
        }

        /// <summary>
        /// Return a path suitable for iterating over collection items in a template.
        /// </summary>
        /// <param name="file">A FileInfo object derived from the path of the file.</param>
        /// <returns>
        /// A string the represents a relative path from the project's site directory to the
        /// output location of this file.
        /// </returns>
        public static string GetIterablePath(FileInfo file)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.Name);
            return $"/{file.Directory.Name}/{fileName}/";
        }
    }
}