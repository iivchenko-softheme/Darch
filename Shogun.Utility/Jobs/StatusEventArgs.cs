// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;

namespace Shogun.Utility.Jobs
{
    /// <summary>
    /// Provides data for the event that is raised when the <see cref="IJob"/> JobStatus changed.
    /// </summary>
    public class StatusEventArgs : EventArgs
    {
        public StatusEventArgs(JobStatus status)
        {
            Status = status;
        }

        public JobStatus Status { get; private set; }
    }
}
