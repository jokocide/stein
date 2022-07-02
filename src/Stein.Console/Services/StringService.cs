using System;

namespace Stein.Services
{
    /// <summary>
    /// Helper methods for interacting with strings.
    /// </summary>
    public static class StringService
    {
        /// <summary>
        /// Print text with the specified color.
        /// </summary>
        /// <param name="text">The desired text output.</param>
        /// <param name="color">The desired color of text.<param>
        public static void Colorize(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
        
        /// <summary>
        /// Print text with the specified color, followed by a newline.
        /// </summary>
        /// <param name="text">The desired text output.</param>
        /// <param name="color">The desired color of text.<param>
        public static void ColorizeLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Print text in bold using ASCII escape sequences, with no newline.
        /// </summary>
        /// <param name="text">The desired text output.</param>
        public static void Bold(string text)
        {
            Console.Write($"\x1b[1m{text}\x1b[0m");
        }

        /// <summary>
        /// Print text in bold using ASCII escape sequences, followed by a newline.
        /// </summary>
        /// <param name="text">The desired text output.</param>
        public static void BoldLine(string text)
        {
            Console.WriteLine($"\x1b[1m{text}\x1b[0m");
        }

        /// <summary>
        /// Return the string between the starting and ending indices.
        /// </summary>
        /// <param name="startIndex">
        /// Indicates where the slice should begin. The result will include the character at this index.
        /// </param>
        /// <param name="endIndex">
        /// Indicates where the slice should end. The result will not include the character at this index.
        /// </param>
        /// <param name="text">The source text.</param>
        public static string Slice(int startIndex, int endIndex, string text)
        {
            if (endIndex < 0)
                endIndex = text.Length + endIndex;

            int len = endIndex - startIndex;
            return text.Substring(startIndex, len).Trim();
        }

        /// <summary>
        /// Return a slug representation of text.
        /// </summary>
        /// <returns>Returns a lowercase string where whitespace has been replaced with a hyphen.</returns>
        public static string Slugify(string text)
        {
            text = text.ToLower().Trim();
            return text.Replace(" ", "-");
        }

        /// <summary>
        /// Return a camel case representation of text.
        /// </summary>
        /// <returns>
        /// A string where all whitespace is removed and the first character of every 
        /// word except the first is capitalized.
        /// </returns>
        public static string Camelize(string text)
        {
            text = Squash(text);

            if (Char.IsLower(text[0])) return text;

            char firstCharacter = text[0];
            return text.Remove(0, 1).Insert(0, firstCharacter.ToString().ToLower());
        }

        /// <summary>
        /// Return a pascal case representation of text.
        /// </summary>
        /// <returns>
        /// A string where all whitespace is removed and the first character of every
        /// word is capitalized.
        /// </returns>
        public static string Pascalize(string text)
        {
            text = Squash(text);

            if (Char.IsUpper(text[0])) return text;

            char firstCharacter = text[0];
            return text.Remove(0, 1).Insert(0, firstCharacter.ToString().ToUpper());
        }

        private static string Squash(string text)
        {
            text = text.Trim();

            while (text.Contains(" "))
            {
                int whitespaceIndex = text.IndexOf(" ");
                char characterAfterWhitespace = text[whitespaceIndex + 1];
                text = text.Remove(whitespaceIndex, 2).Insert(whitespaceIndex, characterAfterWhitespace.ToString().ToUpper());
            }

            return text;
        }
    }
}