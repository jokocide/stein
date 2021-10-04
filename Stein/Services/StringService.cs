using System;

namespace Stein.Services
{
    /// <summary>
    /// Helper methods for interacting with strings.
    /// </summary>
    public static class StringService
    {
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
        /// Print text with the specified 
        /// </summary>
        /// <param name="text">The desired output.</param>
        /// <param name="color">The desired color.<param>
        /// <param name="newLine">Determines if a new line will be printed after the text.</param>
        public static void Colorize(string text, ConsoleColor color, bool newLine)
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
        /// <returns></returns>
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
        /// <returns>A string where </returns>
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