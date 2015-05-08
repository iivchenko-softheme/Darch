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
        private readonly IRepository<MapRecord, ulong> _mapRepository;
        private readonly IRepository<MetadataItem, ulong> _metadataRepository;
        private readonly IRepository<byte[], ulong> _dataRepository;

        // TODO: I hope that in future the cahce will no longer be necessary. I wish to create CachedRepository based on IRepository
        private readonly Lazy<IDictionary<byte[], ulong>> _metadataCache;
        private readonly object _lock;

        private bool _disposed;

        public Storage(
            IHash hash,
            IRepository<MapRecord, ulong> mapRepository,
            IRepository<MetadataItem, ulong> metadataRepository,
            IRepository<byte[], ulong> dataRepository)
        {
            _hash = hash;
            _mapRepository = mapRepository;
            _metadataRepository = metadataRepository;
            _dataRepository = dataRepository;

            _metadataCache = new Lazy<IDictionary<byte[], ulong>>(() => _metadataRepository.All.ToDictionary(key => key.Item.Checksum, value => value.Id, new ByteArrayEquityComparer()));
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
                    return _mapRepository
                        .All
                        .Select(x => x.Item.MapId)
                        .Distinct()
                        .ToList();
                }
            }
        }

        // TODO: Change to IReadOnlyCollection
        // TODO: Rename to TakeMap
        public IEnumerable<ulong> ReadMap(ulong mapId)
        {
            lock (_lock)
            {
                return _mapRepository
                    .All
                    .Where(x => x.Item.MapId == mapId)
                    .Select(x => x.Item.RecordIndex)
                    .ToList();
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

                ulong index = 0;

                var indexes = _mapRepository.All.Where(x => x.Item.MapId == mapId).ToList();

                if (indexes.Any())
                {
                    index = indexes.Max(x => x.Item.RecordIndex) + 1;
                }

                var mapRecordItem = new MapRecord(mapId, metadataId, index);

                _mapRepository.Add(mapRecordItem);
            }
        }

        // TODO: rename to TakeBlock
        public byte[] ReadBlockItem(ulong mapId, ulong blockIndex)
        {
            lock (_lock)
            {
                var metadataId = _mapRepository.All.Single(x => x.Item.MapId == mapId && x.Item.RecordIndex == blockIndex).Item.BlockId;
                var metadataItem = _metadataRepository.GetById(metadataId);
                var dataItem = _dataRepository.GetById(metadataItem.DataId);

                return dataItem.Take((int)metadataItem.Size).ToArray();
            }
        }

        // TODO: Rename to RemoveBlock
        public void DeleteBlockItem(ulong mapId, ulong blockIndex)
        {
            lock (_lock)
            {
                // TODO: Check if input exists
                var mapRecord = _mapRepository.All.Single(x => x.Item.MapId == mapId && x.Item.RecordIndex == blockIndex);
                var metadataId = mapRecord.Item.BlockId;
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

                _mapRepository.Delete(mapRecord.Id);
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
                _mapRepository.Dispose();
                _metadataRepository.Dispose();
                _dataRepository.Dispose();
            }
        }
    }
}
