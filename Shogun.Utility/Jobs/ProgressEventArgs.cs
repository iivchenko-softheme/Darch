// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;

namespace Shogun.Utility.Jobs
{
    /// <summary>
    /// Provides data for the event that is raised when the <see cref="IJob"/> progress changed.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(Progress progress)
        {
            Progress = progress;
        }

        public Progress Progress { get; private set; }
    }
}
