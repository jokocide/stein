using System.IO;
using System.Text.Json;
using Stein.Models;

namespace Stein.Services
{
    /// <summary>
    /// Read a stein.json file into memory and store the deserialized contents.
    /// </summary>
    public static class ConfigurationService
    {
        /// <summary>
        /// The currently loaded configuration.
        /// </summary>
        public static Configuration Set { get; set; }

        /// <summary>
        /// Return a serialized representation of the default configuration of a project.
        /// </summary>
        /// <returns>A string representing the default configuration of a Stein project.</returns>
        public static string Defaults => JsonSerializer.Serialize(new Configuration(), Options);

        /// <summary>
        /// The options used during JSON deserialization.
        /// </summary>
        private static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
        {
            // Users have the freedom to use whatever case style they want.
            PropertyNameCaseInsensitive = true,

            // Camel case is the default style.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            // Allows the Defaults property to be formatted in a human-readable way.
            WriteIndented = true
        };

        /// <summary>
        /// Attempt to read a stein.json file in the current directory before any of the properties
        /// are accessed.
        /// </summary>
        static ConfigurationService()
        {
            ReadConfiguration();
        }

        /// <summary>
        /// Deserialize a stein.json file in the current directory and store
        /// the result within the Options property, or use the defaults if no 
        /// stein.json file exists.
        /// </summary>
        private static void ReadConfiguration()
        {
            if (File.Exists("stein.json"))
            {
                string rawJson = File.ReadAllText("stein.json");

                // If the file contains invalid JSON, we will log a message and proceed with defaults.
                try
                {
                    Set = JsonSerializer.Deserialize<Configuration>(rawJson, Options);
                }
                catch (JsonException)
                {
                    MessageService.Log(Message.InvalidJson(new FileInfo("stein.json")));
                }
            }
            else
            {
                Set = new Configuration();
            }
        }
    }
}