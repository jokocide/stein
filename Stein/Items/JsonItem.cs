using Stein.Models;
using Stein.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Stein.Items
{
    /// <summary>
    /// Represents an Item derived from a Markdown file.
    /// </summary>
    public sealed class JsonItem : Item
    {
        /// <summary>
        /// Initialize a new instance of MarkdownItem with the given FileInfo and populate all
        /// available properties within the instance.
        /// </summary>
        public JsonItem(FileInfo fileInfo) : base(fileInfo)
        {
            string rawFile = PathService.ReadAllSafe(Info.FullName);

            if (String.IsNullOrEmpty(rawFile))
                return;

            Dictionary<string, string> rawPairs = new JsonService().Deserialize(rawFile);

            foreach (var (key, value) in rawPairs)
            {
                string point = key.ToLower();
                if (point == "date")
                    Date = value;
                else if (point == "template")
                    Template = value;
                else
                    Data.Add(key, value);
            }

            if (Template == null)
            {
                Message.Log(Message.NoTemplateKey(Info));
                return;
            }
        }

        /// <summary>
        /// Initialize a new instance of SerializedItem and make the properties of this instance of
        /// MarkdownItem available within the dynamic container of that new instance.
        /// </summary>
        /// <returns>
        /// Returns a new Instance of SerializedItem, containing a dynamic object where all 
        /// properties are available at the top level.
        /// </returns>
        public override SerializedItem Serialize()
        {
            dynamic serializedItem = new SerializedItem();
            SerializedItem castedItem = (SerializedItem)serializedItem;

            serializedItem.Link = Link;
            serializedItem.Date = Date;
            serializedItem.Slug = Slug;

            foreach (KeyValuePair<string, string> pair in Data)
            {
                castedItem.Add(pair.Key, pair.Value);
            }

            return serializedItem;
        }

        private Dictionary<string, string> Data { get; } = new Dictionary<string, string>();

    }
}