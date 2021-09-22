﻿using System.Collections.Generic;
using System.Dynamic;

namespace Stein.Models
{
    public class SerializedItem : DynamicObject
    {
        public Dictionary<string, object> Pairs => _dictionary;

        public void Add(string key, object value) => _dictionary.Add(key.ToLower(), value);

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return _dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name.ToLower()] = value;
            return true;
        }

        private Dictionary<string, object> _dictionary = new();
    }
}