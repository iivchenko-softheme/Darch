// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Darch.Deduplication.Storages;
using Darch.ViewModels.Archiving;
using Darch.ViewModels.Utility;
using Shogun.Utility.Jobs;

namespace Darch.ViewModels.ViewModels
{
    public sealed class MainViewModel : BaseViewModel, IMainViewModel
    {
        private readonly IArchiveProvider _archiveProvider;

        private IArchive _archive;
        private IJob _processor;

        private ViewModelStatus _status;
        
        private ulong _totalWork;
        private ulong _workDone;

        private string _name;
        private int _size;
        private int _unpackSize;

        public MainViewModel()
        {
            _archiveProvider = new ArchiveProvider(new MD5Hash());
        }

        public ViewModelStatus Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;

                OnPropertyChanged();
            }
        }

        // TODO: Think on better implementation.
        public IEnumerable<ArchiveFile> Files
        {
            get
            {
                return _archive == null ? new Collection<ArchiveFile>() : _archive.Files;
            }
        }

        public ulong TotalWork
        {
            get
            {
                return _totalWork;
            }

            set
            {
                _totalWork = value;

                OnPropertyChanged();
            }
        }

        public ulong WorkDone
        {
            get
            {
                return _workDone;
            }

            set
            {
                _workDone = value;

                OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;

                OnPropertyChanged();
            }
        }

        public int Size
        {
            get
            {
                return _size;
            }

            set
            {
                _size = value;

                OnPropertyChanged();
            }
        }

        public int UnpackSize
        {
            get
            {
                return _unpackSize;
            }

            set
            {
                _unpackSize = value;

                OnPropertyChanged();
            }
        }

        public void Create(string path)
        {
            if (_archive != null)
            {
                _archive.Close();
            }

            _archive = _archiveProvider.Create(path);
            
            Name = path;
        }

        public void Open(string path)
        {
            Thread.Sleep(10000);
            if (_archive != null)
            {
                _archive.Close();
            }

            _archive = _archiveProvider.Open(path);

            Name = path;
            OnPropertyChanged("Files");
        }

        public void Add(string path)
        {
            _processor = _archive.Add(path);

            Status = ViewModelStatus.Busy;

            _processor.StatusChanged += OnCompleted;
            _processor.ProgressChanged += OnProgress;

            _processor.Start();
        }

        public void Remove(string name)
        {
            _processor = _archive.Remove(name);

            Status = ViewModelStatus.Busy;

            _processor.StatusChanged += OnCompleted;
            _processor.ProgressChanged += OnProgress;

            _processor.Start();
        }

        public void Extract(string name)
        {
            _processor = _archive.Extract(name);

            Status = ViewModelStatus.Busy;

            _processor.StatusChanged += OnCompleted;
            _processor.ProgressChanged += OnProgress;

            _processor.Start();
        }

        private void OnProgress(object sender, ProgressEventArgs e)
        {
            TotalWork = e.Progress.WorkTotal;
            WorkDone = e.Progress.WorkDone;
        }

        private void OnCompleted(object sender, StatusEventArgs e)
        {
            if (e.Status == JobStatus.Succeeded)
            {
                _processor = null;
                _archive.Flush();

                Status = ViewModelStatus.Idle;

                OnPropertyChanged("Files");
            }
        }
    }
}
