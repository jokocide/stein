﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Dagger.Models
{
    /// <summary>
    /// Base class for all Resource types. A Resource represents a file that has been discovered in a project,
    /// and provides properties and methods to store and manipulate the data in that file.
    /// </summary>
    public abstract class Resource
    {
        private string _template;
        
        /// <summary>
        /// Classes deriving from Resource are responsible for providing a method to retrieve data from that
        /// type of resource.
        /// </summary>
        internal abstract void Process();

        /// <summary>
        /// Return all data from a resource in a format suitable for template injection.
        /// </summary>
        /// <returns></returns>
        internal abstract Injectable Serialize();
        
        /// <summary>
        /// Stores the name of a requested template file, not including the extension.
        /// </summary>
        internal string Template
        {
            get => _template;
            set
            {
                if (Path.HasExtension(value))
                    throw new InvalidOperationException("Received path with extension.");

                _template = value;
            }
        }

        /// <summary>
        /// A string representing the output path for a file, injected to allow for generating links
        /// during iteration in a template.
        /// </summary>
        internal string Link { get; set; }
        
        /// <summary>
        /// User-provided string to represent a date, used during sorting if it can be parsed into a
        /// DateTime object.
        /// </summary>
        internal string Date { get; set; }
        
        /// <summary>
        /// If an error is encountered during Process() this will be true, indicating something is wrong with the
        /// resource. It will not be fully processed or made into a Writable as a consequence. 
        /// </summary>
        protected bool IsInvalid { get; private set; } 
        
        /// <summary>
        /// Provides access to the file that this Resource represents.
        /// </summary>
        protected FileInfo Info { get; }
         
        /// <summary>
        /// Issues found during Process() are recorded here.
        /// </summary>
        protected List<InvalidType> Issues { get; } = new();

        protected Resource(FileInfo fileInfo) => Info = fileInfo;

        /// <summary>
        /// Invalidate the Metadata with a given InvalidType.
        /// </summary>
        /// <param name="type">An InvalidType to describe the cause of invalidation.</param>
        protected void Invalidate(InvalidType type)
        {
            if (!IsInvalid) IsInvalid = true;
            Issues.Add(type);
        }

        protected enum InvalidType
        {
            InvalidFormat,
            TemplateNotFound
        }
    }
}