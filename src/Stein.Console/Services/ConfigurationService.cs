using System.IO;
using System.Text.Json;
using Stein.Models;

namespace Stein.Services
{
    public static class ConfigurationService
    {
        static ConfigurationService()
        {
            Raw = File.Exists("stein.json")
            ? PathService.ReadAllSafe("stein.json")
            : null;

            if (Raw == null) return;

            try 
            {
                Config = JsonSerializer.Deserialize<Configuration>(Raw, JsonService.Options);
            }
            catch (JsonException)
            {
                Config = null;
            }

            if (Config.Engine.StartsWith("."))
                Config.Engine = Config.Engine.Remove(0, 1);
        }

        public static Configuration Config { get; }

        private static string Raw { get; }
    }
}