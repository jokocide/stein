using Stein.Interfaces;
using Stein.Services;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Stein.Models
{
    public class ConfigurationService : ISerializer
    {
        public ConfigurationService() =>
            Raw = File.Exists("stein.json")
            ? File.ReadAllText("stein.json")
            : null;

        public Configuration GetConfig()
        {
            try
            {
                return JsonSerializer.Deserialize<Configuration>(Raw);
            }
            catch (JsonException)
            {
                MessageService.Log(Message.InvalidJson(new FileInfo("stein.json")));
                return null;
            }
        }

        public SerializedItem Serialize()
        {
            dynamic item = new SerializedItem();
            SerializedItem castedItem = (SerializedItem)item;
            Dictionary<string, string> pairs = new JsonService().Deserialize(Raw);

            if (pairs == null) return null;

            foreach(var pair in pairs) castedItem.Add(pair.Key, pair.Value);
            return item;
        }

        private string Raw { get; }
    }
}