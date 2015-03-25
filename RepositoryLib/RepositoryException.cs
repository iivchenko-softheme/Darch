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
    ///  Base for all Repository exceptions.
    /// </summary>
    [Serializable]
    public abstract class RepositoryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the RepositoryException class.
        /// </summary>
        protected RepositoryException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RepositoryException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        protected RepositoryException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RepositoryException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        protected RepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RepositoryException class with serialized data.
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination. </param>
        protected RepositoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
