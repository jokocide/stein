using System;
using System.Collections.Generic;
using Dagger.Data.Models;

namespace Dagger.Services
{
    public class Store
    {
        public List<Dictionary<string, string>> Posts { get; } = new List<Dictionary<string, string>>();
        public List<Writable> Writable { get; } = new List<Writable>();
    }
}