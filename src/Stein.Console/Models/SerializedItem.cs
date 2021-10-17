using System.Collections.Generic;
using System.Dynamic;

namespace Stein.Models
{
    /// <summary>
    /// Represents an object that supports referencing members dynamically.
    /// </summary>
    public class SerializedItem : DynamicObject
    {
        /// <summary>
        /// Contains the actual key/value pairs within this instance.
        /// </summary>
        public Dictionary<string, object> Pairs => _dictionary;

        /// <summary>
        /// Add a key/value pair to the dictionary.
        /// </summary>
        public void Add(string key, object value) => _dictionary.Add(key.ToLower(), value);

        /// <summary>
        /// Called when a member that is not defined in class is accessed.
        /// </summary>
        /// <param name="binder">
        /// Represents the dynamic get member operation at the call site, 
        /// providing the binding semantic and the details about the operation.
        /// </param>
        /// <param name="result">
        /// Contains the requested property, if it is found on this object.
        /// </param>
        /// <returns>Returns true if the requested property is found.</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return _dictionary.TryGetValue(name, out result);
        }

        /// <summary>
        /// Called when you attempt to set a member that is not defined in class.
        /// </summary>
        /// <param name="binder">
        /// Represents the dynamic set member operation at the call site, providing 
        /// the binding semantic and the details about the operation.
        /// </param>
        /// <param name="value">The value to set on binder.</param>
        /// <returns>
        /// You can always set a member on a dynamic object, so this method
        /// always returns true.
        /// </returns>
        /// <remarks>
        /// Property names are case insensitive because they are always converted to 
        /// lowercase by this method.
        /// </remarks>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name.ToLower()] = value;
            return true;
        }

        private Dictionary<string, object> _dictionary = new();
    }
}