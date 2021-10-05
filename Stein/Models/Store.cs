using System;
using System.Collections.Generic;

namespace Stein.Models
{
    /// <summary>
    /// Represents a storage space for the objects generated during build step.
    /// </summary>
    public class Store
    {
        /// <summary>
        /// Register an object with the store.
        /// </summary>
        /// <typeparam name="T">A Writable or Collection object.</typeparam>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the store receives a request to register an object that is not a 
        /// Writable or Collection.
        /// </exception>
        public void Register<T>(T item)
        {
            switch (item)
            {
                case Writable writable:
                    Register(writable);
                    break;
                case Collection collection:
                    Register(collection);
                    break;
                default:
                    throw new InvalidOperationException("Attempted to register an invalid type with Store.");
            }
        }

        private void Register(Collection collection) => Collections.Add(collection);

        private void Register(Writable writable) => Writable.Add(writable);

        /// <summary>
        /// Contains the registered Collection objects.
        /// </summary>
        public List<Collection> Collections { get; } = new();

        /// <summary>
        /// Contains the registered Writable objects.
        /// </summary>
        public List<Writable> Writable { get; } = new();
    }
}