// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Threading;
using System.Threading.Tasks;
using Darch.Deduplication.Storages;
using Darch.Deduplication.Utility;
using Shogun.Utility.Extensions;

namespace Darch.Deduplication.Maps
{
    /// <summary>
    /// Infrastructure for different map processors. Provides Status, Progress, Start, Pause, Resume, Cancel etc. API.
    /// </summary>
    public abstract class BaseProcessor : IMapProcessor
    {
        private readonly Task _workTask;
        private readonly ManualResetEvent _pause;

        private MapStatus _status;
        private Progress _progress;
        
        private bool _disposed;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseProcessor"/> class.
        /// </summary>
        /// <param name="mapId">The identifier of the processing map.</param>
        /// <param name="storage">The storage with the data that is handled by the processor.</param>
        protected BaseProcessor(ulong mapId, IStorage storage)
        {
            Id = mapId;
            Storage = storage;

            _disposed = false;

            _pause = new ManualResetEvent(false);
            _workTask = new Task(Execute);
        }

        /// <summary>
        /// Raises when the processor changes it's status.
        /// </summary>
        public event EventHandler<StatusEventArgs> StatusChanged;

        /// <summary>
        /// Raises with the processor changes it's progress.
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Gets map identifier.
        /// </summary>
        public ulong Id { get; private set; }

        /// <summary>
        /// Gets or sets current processor status.
        /// </summary>
        public MapStatus Status
        {
            get
            {
                ThrowIfDisposed();

                return _status;
            }

            protected set
            {
                _status = value;

                OnStatusChanged();
            }
        }

        /// <summary>
        /// Gets or sets current processor progress.
        /// </summary>
        public Progress Progress
        {
            get
            {
                ThrowIfDisposed();

                return _progress;
            }

            protected set
            {
                _progress = value;

                OnProgressChanged();
            }
        }

        /// <summary>
        /// Gets access to the storage so derived processor can use it.
        /// </summary>
        protected IStorage Storage { get; private set; }

        /// <summary>
        /// Starts processor task. If the processor is on pause - continue execution.
        /// </summary>
        public void Start()
        {
            ThrowIfDisposed();
            
            _pause.Set();
            
            if (_workTask.Status == TaskStatus.Created)
            {
                _workTask.Start();
            }

            Status = MapStatus.InProgress;
        }

        /// <summary>
        /// Stops and terminates processor task execution.
        /// </summary>
        public void Cancel()
        {
            ThrowIfDisposed();

            Status = MapStatus.Canceling;
        }

        /// <summary>
        /// Suspend processor task execution.
        /// </summary>
        public void Pause()
        {
            ThrowIfDisposed();

            _pause.Reset();

            Status = MapStatus.Paused;
        }

        /// <summary>
        /// Hangs current thread until processor task finished.
        /// </summary>
        public void Wait()
        {
            ThrowIfDisposed();
            
            _workTask.Wait();
        }

        public void Dispose()
        {
            Dispose(true);

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Specify production code in derived classes.
        /// </summary>
        protected abstract void InternalAction();

        /// <summary>
        /// In case of user pauses the processor the derived class should wait un pause.
        /// </summary>
        protected void WaitResume()
        {
            _pause.WaitOne();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Status == MapStatus.InProgress || Status == MapStatus.Pending)
            {
                Status = MapStatus.Canceling;
                
                _pause.Set();

                _workTask.Wait();
            }

            if (disposing && !_disposed)
            {
                _workTask.Dispose();
                _pause.Dispose();
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("The processor is disposed.");
            }
        }

        private void OnStatusChanged()
        {
            StatusChanged.OnEvent(this, new StatusEventArgs(_status));
        }

        private void OnProgressChanged()
        {
            ProgressChanged.OnEvent(this, new ProgressEventArgs(_progress));
        }

        private void Execute()
        {
            try
            {
                InternalAction();
            }
            catch
            {
                Status = MapStatus.Failed;
                throw;
            }
        }
    }
}
