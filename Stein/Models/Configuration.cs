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

        public string Engine { get; } = "hbs";

        public Configuration GetConfig()
        {
            try
            {
                return JsonSerializer.Deserialize<Configuration>(Raw);
            }
            catch (JsonException)
            {
                Message.Log(Message.InvalidJson(new FileInfo("stein.json")));
                return null;
            }
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
