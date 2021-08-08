using System.IO;

namespace Dagger.Services
{
    /// <summary>
    /// Track important paths and provide methods to interact with and change those paths.
    /// </summary>
    public static class PathService
    {
        public static string ProjectPath { get; } = Directory.GetCurrentDirectory();
        public static string ResourcesPath => Path.Join(ProjectPath, "resources");
        public static string SitePath => Path.Join(ProjectPath, "site");
        public static string PagesPath => Path.Join(ResourcesPath, "pages");
        public static string TemplatesPath => Path.Join(ResourcesPath, "templates");
        public static string CollectionsPath => Path.Join(ResourcesPath, "collections");
        public static string PartialsPath => Path.Join(TemplatesPath, "partials");
        public static string ResourcesPublicPath => Path.Join(ResourcesPath, "public");
        public static string SitePublicPath => Path.Join(SitePath, "public");
        
        /// <summary>
        /// Copy one directory to another, recursion is optional.
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

            if (!dir.Exists)
            {
                string error = $"Source directory does not exist or could not be found: {source}";
                throw new DirectoryNotFoundException(error);
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
        /// Return true if the given path is a Dagger project, defaults to the current
        /// directory if no path is given.
        /// </summary>
        /// <param name="path">A path to the directory that will be tested.</param>
        /// <returns>Return True if the path is a Dagger project.</returns>
        public static bool IsProject(string path = null)
        {
            path ??= Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, ".dagger"));
        }
        
        /// <summary>Return a path representing a suitable output location for the given file.</summary>
        /// <param name="file">A string representing the current location of the file.</param>
        /// <returns>A string that represents a suitable output location.</returns>
        public static string GetOutputPath(FileInfo file)
        {
            string fileNameNoExtension = Path.GetFileNameWithoutExtension(file.Name);
            
            return file.Directory.Name switch
            {
                "pages" when fileNameNoExtension == "index" => IndexPagePath(),
                "pages" => NonIndexPagePath(fileNameNoExtension),
                _ => CollectionPath(fileNameNoExtension, file.Directory.Name)
            };
        }
        
        /// <summary>
        /// Return a path suitable for an index page file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A string that represents a suitable output location.</returns>
        private static string IndexPagePath() => Path.Join(PathService.SitePath, "index.html");

        /// <summary>
        /// Return a path suitable for non-index page files.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>A string that represents a suitable output location.</returns>
        private static string NonIndexPagePath(string fileName) 
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
        private static string CollectionPath(string fileName, string directoryName)
        {
            string newDirectory = Path.Join(PathService.SitePath, directoryName, fileName);
            Directory.CreateDirectory(newDirectory);
            return Path.Join(newDirectory, "index.html");
        }
    }
}