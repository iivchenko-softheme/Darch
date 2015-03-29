// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

// TODO: Do this guys internal
namespace Deduplication
{
    /// <summary>
    /// Displays data handling process.
    /// </summary>
    public class Progress
    {
        /// <summary>
        /// Gets the entire amount or work (in bytes) to do.
        /// </summary>
        public ulong WorkTotal { get; set; }

        /// <summary>
        /// Gets the amount or work (in bytes) that is already done.
        /// </summary>
        public ulong WorkDone { get; set; }
    }
}
