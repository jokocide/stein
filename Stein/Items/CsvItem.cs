using System;
using System.IO;
using Stein.Interfaces;
using Stein.Models;

namespace Stein.Collections
{
    public sealed class CsvItem : Item, ISerializable
    {
        public CsvItem(FileInfo fileInfo) : base(fileInfo) { }

        public SerializedItem Serialize()
        {
            throw new NotImplementedException();
        }

        internal override void Process(Store store)
        {
            throw new NotImplementedException();
        }
    }
}