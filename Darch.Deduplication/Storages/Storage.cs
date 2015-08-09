// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Linq;
using Darch.Deduplication.Utility;
using Shogun.Patterns.Repositories;

namespace Darch.Deduplication.Storages
{
    // TODO: This will be the Reposiry so rename it after project migration.
    // TODO: add parameters validation.
    // TODO: Googld info about when it is better to use struct than class. And may be use struct.
    public sealed class Storage : IStorage
    {
        private readonly IHash _hash;
        private readonly IMapProvider _mapProvider;
        private readonly IRepository<MetadataItem, ulong> _metadataRepository;
        private readonly IRepository<byte[], ulong> _dataRepository;

        // TODO: I hope that in future the cahce will no longer be necessary. I wish to create CachedRepository based on IRepository
        private readonly Lazy<IDictionary<byte[], ulong>> _metadataCache;
        private readonly object _lock;

        private bool _disposed;

        public Storage(
            IHash hash,
            IMapProvider mapProvider,
            IRepository<MetadataItem, ulong> metadataRepository,
            IRepository<byte[], ulong> dataRepository)
        {
            _hash = hash;
            _mapProvider = mapProvider;
            _metadataRepository = metadataRepository;
            _dataRepository = dataRepository;

            _metadataCache = new Lazy<IDictionary<byte[], ulong>>(() =>
            {
                var dictionary = new Dictionary<byte[], ulong>(new ByteArrayEquityComparer());

                foreach (var id in _metadataRepository.Ids)
                {
                    var item = _metadataRepository.GetById(id);

                    dictionary.Add(item.Checksum, id);
                }

                return dictionary;
            });

            _lock = new object();

            _disposed = false;
        }

        // TODO: Change to IReadOnlyCollection
        public IEnumerable<ulong> MapIds
        {
            get
            {
                lock (_lock)
                {
                    // TODO: Chek on locable if several threads are working
                    return _mapProvider.Ids;
                }
            }
        }

        // TODO: Change to IReadOnlyCollection
        // TODO: Rename to TakeMap
        public IEnumerable<ulong> ReadMap(ulong mapId)
        {
            lock (_lock)
            {
                return _mapProvider[mapId].Ids;
            }
        }

        // TODO: rename to AddBlock
        public void AddBlockItem(ulong mapId, byte[] block, int realSize)
        {
            lock (_lock)
            {
                var hash = _hash.Calculate(block);

                ulong metadataId;

                if (_metadataCache.Value.TryGetValue(hash, out metadataId))
                {
                    var item = _metadataRepository.GetById(metadataId);

                    item.References++;

                    _metadataRepository.Update(metadataId, item);
                }
                else
                {
                    var dataId = _dataRepository.Add(block);

                    var item = new MetadataItem
                    {
                        DataId = dataId,
                        Checksum = hash,
                        References = 1,
                        Size = (ulong)realSize
                    };

                    metadataId = _metadataRepository.Add(item);

                    _metadataCache.Value.Add(hash, metadataId);
                }

                var mapRecordItem = new MapRecord(metadataId);

                _mapProvider[mapId].Add(mapRecordItem);
            }
        }

        // TODO: rename to TakeBlock
        public byte[] ReadBlockItem(ulong mapId, ulong id)
        {
            lock (_lock)
            {
                var metadataId =
                    _mapProvider[mapId].GetById(id)
                    .BlockId;

                var metadataItem = _metadataRepository.GetById(metadataId);
                var dataItem = _dataRepository.GetById(metadataItem.DataId);

                return dataItem.Take((int)metadataItem.Size).ToArray();
            }
        }

        // TODO: Rename to RemoveBlock
        public void DeleteBlockItem(ulong mapId, ulong id)
        {
            lock (_lock)
            {
                // TODO: Check if input exists
                var item = _mapProvider[mapId].GetById(id);

                var metadataId = item.BlockId;
                var metadataItem = _metadataRepository.GetById(metadataId);

                if (metadataItem.References > 1)
                {
                    metadataItem.References--;

                    _metadataRepository.Update(metadataId, metadataItem);
                }
                else
                {
                    _metadataRepository.Delete(metadataId);
                    _dataRepository.Delete(metadataItem.DataId);
                }

                _mapProvider[mapId].Delete(id);
            }
        }

        public void Dispose()
        {
            Dispose(true);

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _mapProvider.Dispose();
                _metadataRepository.Dispose();
                _dataRepository.Dispose();
            }
        }
    }
}
