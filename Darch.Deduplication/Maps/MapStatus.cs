// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Darch.Deduplication.Maps
{
    /// <summary>
    /// Set of data handling statuses.
    /// </summary>
    public enum MapStatus
    {
        Pending,
        InProgress,
        Paused,
        Failed,
        Canceling,
        Canceled,
        Succeeded
    }
}
