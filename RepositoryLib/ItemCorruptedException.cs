// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Runtime.Serialization;

namespace RepositoryLib
{
    /// <summary>
    /// The exception that is thrown when a Repository item is corrupted in any way.
    /// </summary>
    [Serializable]
    public sealed class ItemCorruptedException : RepositoryException
    {
        public ItemCorruptedException()
        {
        }

        public ItemCorruptedException(string message) 
            : base(message)
        {
        }

        public ItemCorruptedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        private ItemCorruptedException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
