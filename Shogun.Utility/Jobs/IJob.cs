// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;

namespace Shogun.Utility.Jobs
{
    /// <summary>
    /// Provides functionality to implement some process handler and monitor.
    /// </summary>
    public interface IJob : IDisposable
    {
        /// <summary>
        /// Raises when the <see cref="Status"/> of the <see cref="IJob"/> has been changed.
        /// </summary>
        event EventHandler<StatusEventArgs> StatusChanged;

        /// <summary>
        /// Raises when the <see cref="Progress"/> of the <see cref="IJob"/> has been changed.
        /// </summary>
        event EventHandler<ProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Gets the identifier of the <see cref="IJob"/>.
        /// </summary>
        ulong Id { get; }
        
        /// <summary>
        /// Gets the <see cref="IJob"/> status.
        /// </summary>
        JobStatus Status { get; } 

        /// <summary>
        /// Gets the <see cref="IJob"/> progress.
        /// </summary>
        Progress Progress { get; } 
        
        /// <summary>
        /// Begins or continues <see cref="IJob"/> execution.
        /// </summary>
        void Start();

        /// <summary>
        /// Cancels <see cref="IJob"/> execution.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Pauses <see cref="IJob"/> execution.
        /// </summary>
        void Pause();

        /// <summary>
        /// Waits for the <see cref="IJob"/> to complete execution.
        /// </summary>
        void Wait();
    }
}
