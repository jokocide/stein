using System;
using System.IO;
using Stein.Interfaces;
using Stein.Models;

namespace Stein.Collections
{
    public sealed class XmlItem : Item, ISerializable
    {
        public XmlItem(FileInfo fileInfo) : base(fileInfo) { }

        public SerializedItem Serialize() => throw new NotImplementedException();

        public override Writable Process() => throw new NotImplementedException();
    }
}