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
        private static string _headerSeparator { get; } = "------------------------------";

        /*
        Return true if the path is a Dagger project as indicated by the prescence 
        of a .dagger file. Defaults to the current directory with no given path.
        */
        public static bool CheckIsProject(string path = null)
        {
            if (path == null)
            {
                path = Directory.GetCurrentDirectory();
            }

            bool isDaggerProject = File.Exists(Path.Join(path, ".dagger"));
            return isDaggerProject;
        }

        // Prints out the command that was called.
        public static void PrintArguments(string[] args = null)
        {
            Console.WriteLine(); // Display empty line for clarity.

            if (args.Length > 0)
            {
                Colorize(ConsoleColor.Blue, "dagger ", false);
                Colorize(ConsoleColor.Gray, String.Join(' ', args));
            }
            else
            {
                Colorize(ConsoleColor.Blue, "dagger");
            }

            Colorize(ConsoleColor.DarkGray, _headerSeparator); // Display visible separator before the routine is executed.
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
