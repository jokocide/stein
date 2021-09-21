using Stein.Interfaces;
using Stein.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace Stein.Services
{
    public class JsonService : IInterpreter
    {
        public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public Dictionary<string, string> Deserialize(string text)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(text, Options);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public string Serialize(Configuration config)
        {
            return JsonSerializer.Serialize(config, Options);
        }
    }
}
