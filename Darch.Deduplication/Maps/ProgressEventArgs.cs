// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;

namespace Darch.Deduplication.Maps
{
    /// <summary>
    /// Provides data for the event that is raised when data handling progress changed.
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
