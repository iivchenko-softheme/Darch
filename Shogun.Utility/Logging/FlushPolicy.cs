// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Shogun.Utility.Logging
{
    /// <summary>
    /// Defines the way how the <see cref="Logger"/> displays messages.
    /// </summary>
    public enum FlushPolicy
    {
        /// <summary>
        /// Writes messages to the output.
        /// </summary>
        Instant,

        /// <summary>
        /// Stores messages. Writers them when <see cref="Logger.Flush"/> is called.
        /// </summary>
        Buffered
    }
}
