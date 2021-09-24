using System;

namespace Stein.Services
{
    public static class StringService
    {
        public static string Slice(int startIndex, int endIndex, string text)
        {
            if (endIndex < 0)
                endIndex = text.Length + endIndex;

            int len = endIndex - startIndex;
            return text.Substring(startIndex, len).Trim();
        }

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

        public static string Slugify(string text)
        {
            text = text.ToLower().Trim();
            return text.Replace(" ", "-");
        }

        public static string Camelize(string text)
        {
            text = Squash(text);

            if (Char.IsLower(text[0])) return text;

            char firstCharacter = text[0];
            return text.Remove(0, 1).Insert(0, firstCharacter.ToString().ToLower());
        }

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