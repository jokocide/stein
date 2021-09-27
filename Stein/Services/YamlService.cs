using Stein.Interfaces;
using System;
using System.Collections.Generic;

namespace Stein.Services
{
    public class YamlService : IDeserializer
    {
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