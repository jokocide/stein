using Stein.Interfaces;
using System;
using System.Collections.Generic;

namespace Stein.Services
{
    public class YamlService : IInterpreter
    {
        public (int FirstStart, int FirstEnd, int SecondStart, int SecondEnd) GetIndicatorIndices(string text)
        {
            int firstStart = text.IndexOf("---", StringComparison.Ordinal);
            int firstEnd = firstStart + 3;
            int secondStart = text.IndexOf("---", firstEnd, StringComparison.Ordinal);
            if (secondStart == -1) return (0, 0, 0, 0);
            int secondEnd = secondStart + 3;
            return (firstStart, firstEnd, secondStart, secondEnd);
        }

        public Dictionary<string, string> Deserialize(string text)
        {
            Dictionary<string, string> dictionary = new();

            string[] lines = text.Split(Environment.NewLine);
            foreach (string line in lines)
            {
                string[] splitLines = line.Split(":", 2);
                string key = splitLines[0].Trim();

                // Keys with whitespace are camel cased by default!
                if (key.Contains(" ")) key = StringService.Camelize(key);

                string value = splitLines[1].Trim();
                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }
}