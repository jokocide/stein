using System;
using System.IO;

namespace Dagger.Services
{
    /// <summary>
    /// Static methods for common actions. If an action is only ever required by on specific
    /// service, it should be defined within that class as a private method instead of going here.
    /// </summary>
    public static class Helper
    {
        private static string Separator { get; } = "----->";

        /// <summary>
        /// Return true if the given path is a Dagger project. Looks in the given path for a .dagger file, defaults
        /// to the current directory with no given path.
        /// </summary>
        public static bool CheckIsProject(string path = null)
        {
            if (path == null) path = Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, ".dagger"));
        }

        /// <summary>
        /// Prints out Dagger's 'header' and any received arguments for clarity.
        /// </summary>
        public static void PrintArguments(string[] args = null)
        {
            Console.WriteLine(); // Display empty line for clarity.
            
            Colorize(ConsoleColor.Cyan, "Dagger ", false);

            if (args != null)
                Colorize(ConsoleColor.Gray, String.Join(' ', args));

            Colorize(ConsoleColor.Cyan, Separator); // Display visible separator before the routine is executed.
        }

        /// <summary>
        /// Synchronize two directories by copying all files from sourceDirName to destDirName. Will also copy directories
        /// if copySubDirs is true.
        /// </summary>
        public static void Synchronize(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDirName);        

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    Synchronize(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        /// <summary>
        /// Print a given text string with the foreground set to the given ConsoleColor. Resets the color of the
        /// text back to default after printing.
        /// </summary>
        public static void Colorize(ConsoleColor color, string text, bool newLine = true)
        {
            Console.ForegroundColor = color;

            if (newLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Return a substring of text starting at the first index and ending at, but not including, the second index.
        /// </summary>
        /// <param name="startIndex">The starting index.</param>
        /// <param name="endIndex">The ending index.</param>
        /// <param name="text">The source text.</param>
        /// <returns>A string of text between the two given indices.</returns>
        public static string Slice(int startIndex, int endIndex, string text)
        {
            if (endIndex < 0)
            {
                endIndex = text.Length + endIndex;
            }

            int len = endIndex - startIndex;
            return text.Substring(startIndex, len);
        }
    }
}
