// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;

namespace Darch.Deduplication.Maps
{
    /// <summary>
    /// Provides functionality to monitor data handling process 
    /// like adding data to a repository 
    /// or reading data from the repository
    /// or deleting data form the repository.
    /// </summary>
    public interface IMapProcessor : IDisposable
    {
        /// <summary>
        /// Raises when <see cref="IMapProcessor"/> <see cref="Status"/> has been changed.
        /// </summary>
        event EventHandler<StatusEventArgs> StatusChanged;

        /// <summary>
        /// Raises when <see cref="IMapProcessor"/> <see cref="Progress"/> has been changed.
        /// </summary>
        event EventHandler<ProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Gets the identifier of the map that logically combines piece of data.
        /// </summary>
        ulong Id { get; }
        
        /// <summary>
        /// Gets the data handling status.
        /// </summary>
        MapStatus Status { get; } 

        /// <summary>
        /// Gets the data handling progress.
        /// </summary>
        Progress Progress { get; } 
        
        /// <summary>
        /// Begins or continues data handling process.
        /// </summary>
        void Start();

        /// <summary>
        /// Cancels data handling process.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Pauses data handling process.
        /// </summary>
        void Pause();

        /// <summary>
        /// Waits for the <see cref="IMapProcessor"/> to complete data handling process.
        /// </summary>
        void Wait();
    }
}
