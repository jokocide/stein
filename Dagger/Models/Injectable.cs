using System.Collections.Generic;
using System.Dynamic;

namespace Dagger.Models
{
    /// <summary>
    /// Represents a Resource that has been transformed into a format suitable for template injection.
    /// </summary>
    public class Injectable : DynamicObject
    {
        /// <summary>
        /// Contains the data to be injected into the template.
        /// </summary>
        private Dictionary<string, object> _dictionary = new();
        
        /// <summary>
        /// Insert a string key and object value into this Injectable.
        /// </summary>
        /// <param name="key">The key name that will refer to value.</param>
        /// <param name="value">The desired value.</param>
        public void Add(string key, object value)
        {
            _dictionary.Add(key.ToLower(), value);
        }

        /// <summary>
        /// Called to retrieve a value that is not defined on Injectable.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            return _dictionary.TryGetValue(name, out result);
        }

        /// <summary>
        /// Called to set a value on a property that is not defined on Injectable.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            _dictionary[binder.Name.ToLower()] = value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }
}