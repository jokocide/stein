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
        private static string HeaderSeparator { get; } = "------------------------------";

        /*
        Return true if the path is a Dagger project as indicated by the prescence 
        of a .dagger file. Defaults to the current directory with no given path.
        */
        public static bool CheckIsProject(string path = null)
        {
            if (path == null) path = Directory.GetCurrentDirectory();
            return File.Exists(Path.Join(path, ".dagger"));
        }

        // Prints out the command that was called.
        public static void PrintArguments(string[] args = null)
        {
            Console.WriteLine(); // Display empty line for clarity.
            Colorize(ConsoleColor.Cyan, HeaderSeparator); // Display visible separator before the routine is executed.

            if (args.Length > 0)
            {
                Colorize(ConsoleColor.Cyan, "Dagger ", false);
                Colorize(ConsoleColor.Gray, String.Join(' ', args));
            }
            else
            {
                Colorize(ConsoleColor.Blue, "dagger");
            }

            Colorize(ConsoleColor.Cyan, HeaderSeparator); // Display visible separator before the routine is executed.
        }

        // Synchronize two directories, recursive functionality is optional.
        public static void Synchronize(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
        
            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);        

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    Synchronize(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        /* 
        Allows text to be printed in a color before resetting the color back to default.
        Prints on a newline with Console.WriteLine() by default.
        */
        private static void Colorize(ConsoleColor color, string text, bool newLine = true)
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
    }
}
