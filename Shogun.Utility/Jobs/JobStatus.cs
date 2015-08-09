// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Shogun.Utility.Jobs
{
    /// <summary>
    /// Set of the <see cref="IJob"/> statuses.
    /// </summary>
    public enum JobStatus
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
