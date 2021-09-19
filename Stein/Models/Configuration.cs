using Stein.Interfaces;
using Stein.Services;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Stein.Models
{
    public class Configuration : ISerializable
    {
        public static Configuration LoadedConfiguration { get; }

        public static string DefaultConfiguration => JsonSerializer.Serialize(new Configuration(), JsonService.Options);

        public bool SilenceWarnings { get; set; } = false;

        public SerializedItem Serialize()
        {
            dynamic item = new SerializedItem();
            SerializedItem castedItem = (SerializedItem)item;

            string text = File.ReadAllText("stein.json");

            JsonService service = new();
            Dictionary<string, string> pairs = service.Deserialize(text);

            foreach(var pair in pairs)
            {
                castedItem.Add(pair.Key, pair.Value);
            }

            return item;
        }

        static Configuration()
        {
            string text = File.ReadAllText("stein.json");

            try
            {
                LoadedConfiguration = JsonSerializer.Deserialize<Configuration>(text, JsonService.Options);
            }
            catch (JsonException)
            {
                FileInfo fileInfo = new("stein.json");
                MessageService.Log(Message.InvalidJson(fileInfo));
                LoadedConfiguration = new();
            }
        }
    }
}