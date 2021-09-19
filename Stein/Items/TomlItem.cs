using System;
using System.IO;
using Stein.Interfaces;
using Stein.Models;

namespace Stein.Collections
{
    public sealed class TomlItem : Item, ISerializable
    {
        public TomlItem(FileInfo fileInfo) : base(fileInfo) { }

        public SerializedItem Serialize()
        {
            throw new NotImplementedException();
        }

        public override void Process(Store store)
        {
            throw new NotImplementedException();
        }
    }
}