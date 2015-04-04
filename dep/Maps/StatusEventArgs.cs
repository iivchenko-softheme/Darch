// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;

namespace Deduplication.Maps
{
    /// <summary>
    /// Provides data for the event that is raised when data handling status changed.
    /// </summary>
    public class StatusEventArgs : EventArgs
    {
        public StatusEventArgs(MapStatus status)
        {
            Status = status;
        }

        public MapStatus Status { get; private set; }
    }
}
