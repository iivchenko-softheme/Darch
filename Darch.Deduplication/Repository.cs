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
using Darch.Deduplication.Maps;

namespace Darch.Deduplication
{
    // TODO: Rename it to repository manager
    // TODO: add map management, so one map could be opented for one task at a time
    public sealed class Repository : IRepository
    {
        private readonly IMapProcessorFactory _mapProcessorFactory;
        private readonly int _blockSize;

        private readonly IList<ulong> _mapIdsCahce;
        private readonly object _lock;
        private bool _disposed;

        public Repository(IMapProcessorFactory mapProcessorFactory, IList<ulong> mapIdsCache, int blockSize)
        {
            _mapProcessorFactory = mapProcessorFactory;
            _blockSize = blockSize;

            _lock = new object();
            _disposed = false;

            _mapIdsCahce = mapIdsCache; // TODO: Move map ids cachin on the level below => Storage
        }

        public IEnumerable<ulong> Maps
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();

                    return _mapIdsCahce;
                }
            }
        }

        public IMapProcessor Write(Stream source)
        {
            lock (_lock)
            {
                ThrowIfDisposed();
                
                var mapId = _mapIdsCahce.Any() ? _mapIdsCahce.Max() + 1 : 0;

                _mapIdsCahce.Add(mapId);

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
                EventHandler<StatusEventArgs> statusAction = null;

                var map = _mapProcessorFactory.CreateDeleteProcessor(mapId);
                
                statusAction = (sender, args) =>
                               {
                                   if (args.Status == MapStatus.Succeeded)
                                   {
                                       _mapIdsCahce.Remove(mapId);
                                   }

                                   if (args.Status == MapStatus.Canceled || 
                                       args.Status == MapStatus.Failed ||
                                       args.Status == MapStatus.Succeeded)
                                   {
                                       map.StatusChanged -= statusAction;
                                   }
                               };

                map.StatusChanged += statusAction;

                return map;
            }
        }

        public void Dispose()
        {
            ThrowIfDisposed();

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
            if (!_mapIdsCahce.Contains(mapId))
            {
                var msg = string.Format(CultureInfo.InvariantCulture, "Map with specified id '{0}' doesn't exist in the repository.", mapId);

                throw new MapMissingException(msg);
            }
        }
    }
}
