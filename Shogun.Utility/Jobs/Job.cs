// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Threading;
using System.Threading.Tasks;
using Shogun.Utility.Extensions;

namespace Shogun.Utility.Jobs
{
    /// <summary>
    /// Infrastructure for different jobs. 
    /// Provides Status, Progress, Start, Pause, Resume, Cancel etc. API.
    /// </summary>
    public abstract class Job : IJob
    {
        private readonly Task _workTask;
        private readonly ManualResetEvent _pause;

        private JobStatus _status;
        private Progress _progress;
        
        private bool _disposed;
        
        protected Job()
        {
            _disposed = false;

            _pause = new ManualResetEvent(false);
            _workTask = new Task(Execute);
        }

        public event EventHandler<StatusEventArgs> StatusChanged;

        public event EventHandler<ProgressEventArgs> ProgressChanged;
        
        public ulong Id { get; protected set; }
        
        public JobStatus Status
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

        public void Start()
        {
            ThrowIfDisposed();
            
            _pause.Set();
            
            if (_workTask.Status == TaskStatus.Created)
            {
                _workTask.Start();
            }

            Status = JobStatus.InProgress;
        }

        public void Cancel()
        {
            ThrowIfDisposed();

            Status = JobStatus.Canceling;
        }

        public void Pause()
        {
            ThrowIfDisposed();

            _pause.Reset();

            Status = JobStatus.Paused;
        }

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
            if (Status == JobStatus.InProgress || Status == JobStatus.Pending)
            {
                Status = JobStatus.Canceling;
                
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
                Status = JobStatus.Failed;
                throw;
            }
        }
    }
}
