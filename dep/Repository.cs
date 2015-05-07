// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Deduplication.Maps;
using Deduplication.Storages;

namespace Deduplication
{
    public sealed class Repository : IRepository
    {
        private readonly IStorage _storage;
        private readonly IMapProcessorFactory _mapProcessorFactory;
        private readonly int _blockSize;

        private readonly object _lock;
        private bool _disposed;

        public Repository(IStorage storage, IMapProcessorFactory mapProcessorFactory, int blockSize)
        {
            _storage = storage;
            _mapProcessorFactory = mapProcessorFactory;
            _blockSize = blockSize;

            _lock = new object();
            _disposed = false;
        }

        public IEnumerable<ulong> Maps
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();

                    return _storage.MapIds;
                }
            }
        }

        public IMapProcessor Write(Stream source)
        {
            lock (_lock)
            {
                ThrowIfDisposed();

                var ids = _storage.MapIds.ToList();
                var mapId = ids.Any() ? ids.Max() + 1 : 0; // TODO: Fix

                return _mapProcessorFactory.CreateWriteProcessor(mapId, _blockSize, source);
            }
        }

        public IMapProcessor Read(ulong mapId, Stream target)
        {
            lock (_lock)
            {
                ThrowIfDisposed();
                ThrowIfMapNotExists(mapId);

                return _mapProcessorFactory.CreateReadProcessor(mapId, target);
            }
        }

        public IMapProcessor Delete(ulong mapId)
        {
            lock (_lock)
            {
                ThrowIfDisposed();
                ThrowIfMapNotExists(mapId);

                return _mapProcessorFactory.CreateDeleteProcessor(mapId);
            }
        }

        public void Dispose()
        {
            ThrowIfDisposed();

            _storage.Dispose();

            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("The repository is disposed.");
            }
        }

        private void ThrowIfMapNotExists(ulong mapId)
        {
            if (!_storage.MapIds.Contains(mapId))
            {
                var msg = string.Format(CultureInfo.InvariantCulture, "Map with specified id '{0}' doesn't exist in the repository.", mapId);

                throw new MapMissingException(msg);
            }
        }
    }
}
