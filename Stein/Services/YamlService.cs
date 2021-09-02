using System;
using System.Collections.Generic;

namespace Stein.Services
{
    /// <summary>
    /// Methods to interact with and change YAML content.
    /// </summary>
    public static class YamlService
    {
        /// <summary>
        /// Find the indices representing the end of the first YAML frontmatter indicator and the beginning of the
        /// second YAML frontmatter indicator. all indices between these two numbers represent frontmatter.
        /// </summary>
        /// <param name="text">
        /// A string containing YAML frontmatter.
        /// </param>
        /// <returns>
        /// A tuple representing the beginning and ending indices of the frontmatter content
        /// (without the traditional YAML '---' indicators)
        /// </returns>
        public static (int FirstStart, int FirstEnd, int SecondStart, int SecondEnd) GetIndices(string text)
        {
            int firstStart = text.IndexOf("---", StringComparison.Ordinal);
            int firstEnd = firstStart + 3;
            int secondStart = text.IndexOf("---", firstEnd, StringComparison.Ordinal);
            if (secondStart == -1) throw new ArgumentOutOfRangeException();
            int secondEnd = secondStart + 3;
            return (firstStart, firstEnd, secondStart, secondEnd);
        }

        /// <summary>
        /// Create a MetaData object by dividing key/value pairs by a specified delimiter, or ':' by
        /// default if no delimiter is given.
        /// </summary>
        /// <param name="text">
        /// The source string.
        /// </param>
        /// <param name="delimiter">
        /// The delimiter that will be used to divide the lines.
        /// </param>
        /// <returns>
        /// A new MetaData object containing the key/value pairs from the source string.
        /// </returns>
        public static Dictionary<string, string> Deserialize(string text, string delimiter = ":")
        {
            var dictionary = new Dictionary<string, string>();
            string[] lines = text.Split(Environment.NewLine);
            foreach (string line in lines)
            {
                string[] splitLines = line.Split(delimiter, 2);
                string key = splitLines[0].Trim();
                string value = splitLines[1].Trim();
                dictionary.Add(key, value);
            }
            return dictionary;
        }
    }
}