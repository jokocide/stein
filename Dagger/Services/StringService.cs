using System;

namespace Dagger.Services
{
    public static class StringService
    {
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
    }
}