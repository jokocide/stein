using Stein.Services;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Stein.Models
{
    /// <summary>
    /// Respresents a set of options used to influence various behaviors within routines.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Initialize a new instance of the Configuration class.
        /// </summary>
        /// <remarks>
        /// Reads a stein.json file in the current directory and uses the data in that file to populate
        /// properties on this instance.
        /// </remarks>
        public Configuration()
        {
            Raw = File.Exists("stein.json")
            ? PathService.ReadAllSafe("stein.json")
            : null;
        }

        /// <summary>
        /// Determines if warning (non-critical) messages are shown to the user.
        /// </summary>
        public bool SilenceWarnings { get; set; } = false;

        /// <summary>
        /// Defines the desired templating engine.
        /// </summary>
        /// <remarks>
        /// Currently, only Handlebars (hbs) is supported.
        /// </remarks>
        public string Engine { get; set; } = "hbs";

        /// <summary>
        /// Defines the desired date format.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Return a Configuration object based on the data derived from a stein.json file 
        /// in the current directory.
        /// </summary>
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

        /// <summary>
        /// Return a SerializedItem containing all arbitrary key/value pairs from a stein.json file in 
        /// the current directory.
        /// </summary>
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
