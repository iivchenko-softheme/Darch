// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Runtime.Serialization;

// TODO: Implement serialization and TId type for future
namespace Shogun.Patterns.Repositories
{
    /// <summary>
    /// The exception that is thrown when a Repository doesn't contain item with specified ID.
    /// </summary>
    [Serializable]
    public sealed class MissingItemException : RepositoryException
    {
        public MissingItemException()
        {
        }

        public MissingItemException(string message) 
            : base(message)
        {
        }

        public MissingItemException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        private MissingItemException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
