using Stein.Interfaces;
using Stein.Services;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Stein.Models
{
    public class ConfigurationService : ISerializable
    {
        public ConfigurationService() => Raw = File.ReadAllText("stein.json");

        public Configuration GetConfigOrNull()
        {
            if (string.IsNullOrEmpty(Raw)) return null;

            try
            {
                Configuration config = JsonSerializer.Deserialize<Configuration>(Raw);
                return config;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public Configuration GetConfigOrNew()
        {
            if (string.IsNullOrEmpty(Raw)) return new Configuration();

            try
            {
                Configuration config = JsonSerializer.Deserialize<Configuration>(Raw);
                return config;
            }
            catch (JsonException)
            {
                return new Configuration();
            }
        }

        public SerializedItem Serialize()
        {
            dynamic item = new SerializedItem();
            SerializedItem castedItem = (SerializedItem)item;

            Dictionary<string, string> pairs = new JsonService().Deserialize(Raw);

            foreach(var pair in pairs)
            {
                castedItem.Add(pair.Key, pair.Value);
            }

            return item;
        }

        private string Raw { get; }
    }
}