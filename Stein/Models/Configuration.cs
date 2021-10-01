using Stein.Services;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Stein.Models
{
    public class Configuration
    {
        public Configuration()
        {
            Raw = File.Exists("stein.json")
            ? PathService.ReadAllSafe("stein.json")
            : null;
        }

        public bool SilenceWarnings { get; set; } = false;

        public string Engine { get; set; } = "hbs";

        public Configuration GetConfig()
        {
            Configuration config;

            try
            {
                config = JsonSerializer.Deserialize<Configuration>(Raw, JsonService.Options);
            }
            catch (JsonException)
            {
                return null;
            }

            if (config.Engine.StartsWith("."))
                config.Engine = config.Engine.Remove(0, 1);

            return config;
        }

        public SerializedItem GetConfigAllKeys()
        {
            dynamic item = new SerializedItem();
            SerializedItem castedItem = (SerializedItem)item;
            Dictionary<string, string> pairs = new JsonService().Deserialize(Raw);

            if (pairs == null) return null;

            foreach (var pair in pairs) castedItem.Add(pair.Key, pair.Value);
            return item;
        }

        private string Raw { get; }
    }
}
