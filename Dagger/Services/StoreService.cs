using System.Collections.Generic;
using Dagger.Models;

namespace Dagger.Services
{
    /// <summary>
    /// Keep track of certain object types as they are created and disposed during a Build Routine.
    /// </summary>
    public static class StoreService
    {
        // public Dictionary<string, List<Dictionary<string, string>>> Collections { get; } = new();
        public static List<Collection> Collections { get; } = new();
        public static List<Writable> Writable { get; } = new();

        /// <summary>
        /// Remote all items from the Store.
        /// </summary>
        public static void Clear()
        {
            Collections.Clear();
            Writable.Clear();
        }

        /// <summary>
        /// Generate an injectable dictionary from the contents of all collection items that
        /// are currently registered with the store.
        /// </summary>
        /// <returns>A dictionary containing the key value pairs of all metadata items.</returns>
        // public static Injectable Assemble()
        // {
        //     Dictionary<string, string> assembly = new();
        //
        //     foreach (Collection collection in Collections)
        //     {
        //         if (collection.Items.Count == 0) break;
        //
        //         foreach (var metadata in collection.Items.OfType<MarkdownResource>())
        //         {
        //             metadata.Frontmatter.
        //         }
        //     }
        //     
        // }
    }
}