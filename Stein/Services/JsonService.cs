using Stein.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Stein.Services
{
    public class JsonService
    {
        public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public Dictionary<string, string> Deserialize(string text)
        {
            Dictionary<string, string> dictionary = new();

            try
            {
                using (JsonDocument document = JsonDocument.Parse(text))
                {
                    JsonElement root = document.RootElement;
                    var objectEnum = root.EnumerateObject();

                    foreach(JsonProperty pair in objectEnum)
                    {
                        string key = pair.Name;
                        JsonElement value = pair.Value;

                        dictionary.Add(key, value.ToString());
                    }
                }
            }
            catch
            {
                return null;
            }

            return dictionary;

        }

        public string Serialize(Configuration config)
        {
            try
            {
                return JsonSerializer.Serialize(config, Options);
            }
            catch(NotSupportedException)
            {
                return null;
            }
        }
    }
}
