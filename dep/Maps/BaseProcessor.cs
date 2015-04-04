// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Threading;
using System.Threading.Tasks;
using Deduplication.Storages;

namespace Deduplication.Maps
{
    public abstract class BaseProcessor : IMapProcessor
    {
        private readonly Task _workTask;
        private readonly ManualResetEvent _pause;

        private MapStatus _status;
        private Progress _progress;
        
        private bool _paused;
        private bool _disposed;
        
        protected BaseProcessor(ulong mapId, IStorage storage)
        {
            Id = mapId;
            Storage = storage;

            _disposed = false;

            _pause = new ManualResetEvent(false);
            _workTask = Task.Factory.StartNew(() => InternalAction());
        }

        public event EventHandler<StatusEventArgs> StatusChanged;

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public ulong Id { get; private set; }

        public MapStatus Status
        {
            get
            {
                ThrowIfDisposed();

                return StatusInternal;
            }
        }

        public Progress Progress
        {
            get
            {
                ThrowIfDisposed();

                return ProgressInternal;
            }
        }

        protected MapStatus StatusInternal
        {
            get
            {
                return _status;
            }
            
            set
            {
                _status = value;

                OnStatusChanged(new StatusEventArgs(_status));
            }
        }

        protected Progress ProgressInternal
        {
            get
            {
                return _progress;
            }

            set
            {
                _progress = value;

                OnProgressChanged(new ProgressEventArgs(_progress));
            }
        }

        protected IStorage Storage { get; private set; }

        protected bool Canceled { get; set; }

        protected bool Paused
        {
            get
            {
                return _paused;
            }

            set
            {
                _paused = value;

                if (_paused)
                {
                    _pause.Reset();

                    OnStatusChanged(new StatusEventArgs(MapStatus.Paused));
                }
                else
                {
                    _pause.Set();

                    OnStatusChanged(new StatusEventArgs(MapStatus.InProgress));
                }
            }
        }

        public void Start()
        {
            ThrowIfDisposed();

            Paused = false;
        }

        public void Cancel()
        {
            ThrowIfDisposed();

            Canceled = true;
        }

        public void Pause()
        {
            ThrowIfDisposed();

            Paused = true;
        }

        public void Wait()
        {
            ThrowIfDisposed();

            Paused = false;

            SafeExecute(() => _workTask.Wait());
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected abstract Action InternalAction();

        protected void WaitResume()
        {
            _pause.WaitOne();
        }

        protected void OnStatusChanged(StatusEventArgs e)
        {
            var statusChanged = StatusChanged;

            if (statusChanged != null)
            {
                statusChanged(this, e);
            }
        }

        protected void OnProgressChanged(ProgressEventArgs e)
        {
            var progressChanged = ProgressChanged;

            if (progressChanged != null)
            {
                progressChanged(this, e);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _workTask.Dispose();
                _pause.Dispose();

                _disposed = true;
            }
        }

        private static void SafeExecute(Action action)
        {
            // TODO: Finish with the status changes
            action();
        }
        
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("The repository is disposed.");
            }
        }
    }
}
