using System.IO;

namespace Dagger.Services
{
    public static class PathService
    {
        public static string ProjectPath { get; set; } = Directory.GetCurrentDirectory();
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
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: "
                                                     + source);
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
        /// <param name="path">The path to be evaluated.</param>
        /// <returns>A boolean. True if the path contains a .dagger file, else false.</returns>
        public static bool IsProject(string path = null)
        {
            path ??= Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, ".dagger"));
        }
    }
}