// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Darch.Deduplication.Maps;
using Darch.Deduplication.Storages;
using Shogun.Patterns.Repositories;
using Shogun.Utility.Jobs;

namespace Darch.Deduplication.Tests.Repository
{
    public sealed class ManagedRepository : IRepository
    {
        private readonly IRepository _repository;
        private readonly IStorage _storage;

        public ManagedRepository(int blockSize, int checksumSize, string workDirectory)
        {
            var hash = new MD5Hash();
            
            MetadataStream = File.Open(Path.Combine(workDirectory, Guid.NewGuid() + ".metadata"), FileMode.CreateNew);
            DataStream = File.Open(Path.Combine(workDirectory, Guid.NewGuid() + ".data"), FileMode.CreateNew);

            var mapStreamMapper = new MapStreamMapper();
            var mapProvider = new MapProvider(mapStreamMapper, MapStreamMapper.BufferSize, workDirectory);
            MapProvider = mapProvider;

            var metadataStreamMapper = new MetadataStreamMapper(checksumSize);
            var metadataRepository = new StreamRepository<MetadataItem>(MetadataStream, metadataStreamMapper, metadataStreamMapper.BufferSize);

            var dataStreamMapper = new DataStreamMapper(blockSize);
            var dataRepository = new StreamRepository<byte[]>(DataStream, dataStreamMapper, blockSize);

            _storage = new Storage(hash, mapProvider, metadataRepository, dataRepository);
            var mapProcessorFactory = new MapProcessorFactory(_storage);

            _repository = new Darch.Deduplication.Repository(mapProcessorFactory, _storage.MapIds.ToList(), blockSize);
        }

        public IMapProvider MapProvider { get; private set; }

        public Stream MetadataStream { get; private set; }

        public Stream DataStream { get; private set; }
        
        public IEnumerable<ulong> Maps
        {
            get { return _repository.Maps; }
        }

        public IStorage Storage
        {
            get { return _storage; }
        }

        public IJob Write(Stream source)
        {
            return _repository.Write(source);
        }

        public IJob Read(ulong mapId, Stream target)
        {
           return _repository.Read(mapId, target);
        }

        public IJob Delete(ulong mapId)
        {
            return _repository.Delete(mapId);
        }

        public void Dispose()
        {
            _repository.Dispose();
            _storage.Dispose();
        }
    }
}
