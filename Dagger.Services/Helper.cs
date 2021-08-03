using System;
using System.IO;

namespace Dagger.Services
{
    /// <summary>
    /// Common actions for tasks non-specific to any one Routine.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Return true if the given path is a Dagger project, defaults to the current
        /// directory if no path is given.
        /// </summary>
        /// <param name="projectPath">The path to be evaluated.</param>
        /// <returns>A boolean. True if the path contains a .dagger file, else false.</returns>
        public static bool CheckIsProject(string projectPath = null)
        {
            projectPath ??= Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(projectPath, ".dagger"));
        }

        /// <summary>
        /// Copy one directory to another, recursion is optional.
        /// </summary>
        /// <param name="sourceDirName">A path representing the directory to be copied.</param>
        /// <param name="destDirName">
        /// A path representing the desired location of the copied files from sourceDirName.
        /// </param>
        /// <param name="recursive">
        /// A boolean to control recursive behavior. All subdirectories within sourceDirName
        /// will be copied recursively when true.
        /// </param>
        public static void Synchronize(string sourceDirName, string destDirName, bool recursive = false)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: "
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

            if (!recursive) return;
            
            foreach (DirectoryInfo subDirectory in dirs)
            {
                string tempPath = Path.Combine(destDirName, subDirectory.Name);
                Synchronize(subDirectory.FullName, tempPath, true);
            }
        }

        /// <summary>
        /// Print the given text with the specified ConsoleColor and then set the ConsoleColor
        /// back to default.
        /// </summary>
        /// <param name="color">A ConsoleColor object to specify the desired color of text.</param>
        /// <param name="text">The text to be printed.</param>
        /// <param name="newLine">
        /// Specify if a new line should be printed after the text or not, defaults to true.
        /// </param>
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
        /// Return a substring of text starting at startIndex and ending at but not including secondIndex.
        /// </summary>
        /// <param name="startIndex">The starting index.</param>
        /// <param name="endIndex">The ending index.</param>
        /// <param name="text">The source text.</param>
        /// <returns>A substring of text between the two given indices.</returns>
        public static string Slice(int startIndex, int endIndex, string text)
        {
            if (endIndex < 0)
                endIndex = text.Length + endIndex;

            int len = endIndex - startIndex;
            return text.Substring(startIndex, len);
        }
    }
}
