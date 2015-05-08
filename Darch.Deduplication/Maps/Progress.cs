// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Darch.Deduplication.Maps
{
    /// <summary>
    /// Displays data handling process.
    /// </summary>
    public class Progress
    {
        public Progress(ulong workTotal, ulong workDone)
        {
            WorkTotal = workTotal;
            WorkDone = workDone;
        }

        /// <summary>
        /// Gets the entire amount or work (in bytes) to do.
        /// </summary>
        public ulong WorkTotal { get; internal set; }

        /// <summary>
        /// Gets the amount or work (in bytes) that is already done.
        /// </summary>
        public ulong WorkDone { get; internal set; }
    }
}
