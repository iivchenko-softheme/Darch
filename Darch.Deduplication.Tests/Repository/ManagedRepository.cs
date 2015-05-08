// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Darch.Deduplication.Maps;
using Darch.Deduplication.Storages;
using Shogun.Patterns.Repositories;

namespace Darch.Deduplication.Tests.Repository
{
    public sealed class ManagedRepository : IRepository
    {
        private readonly IRepository _repository;
        private readonly IStorage _storage;

        public ManagedRepository(int blockSize, int checksumSize)
            : this(blockSize, checksumSize, new MemoryStream(), new MemoryStream(), new MemoryStream())
        {
        }

        public ManagedRepository(int blockSize, int checksumSize, Stream mapStream, Stream metadataStream, Stream dataStream)
        {
            var hash = new MD5Hash();

            MapStream = mapStream;
            MetadataStream = metadataStream;
            DataStream = dataStream;

            var mapStreamMapper = new MapStreamMapper();
            var mapRepository = new StreamRepository<MapRecord>(MapStream, mapStreamMapper, MapStreamMapper.BufferSize);

            var metadataStreamMapper = new MetadataStreamMapper(checksumSize);
            var metadataRepository = new StreamRepository<MetadataItem>(MetadataStream, metadataStreamMapper, metadataStreamMapper.BufferSize);

            var dataStreamMapper = new DataStreamMapper(blockSize);
            var dataRepository = new StreamRepository<byte[]>(DataStream, dataStreamMapper, blockSize);

            _storage = new Storage(hash, mapRepository, metadataRepository, dataRepository);
            var mapProcessorFactory = new MapProcessorFactory(_storage);

            _repository = new Darch.Deduplication.Repository(mapProcessorFactory, _storage.MapIds.ToList(), blockSize);
        }

        public Stream MapStream { get; private set; }

        public Stream MetadataStream { get; private set; }

        public Stream DataStream { get; private set; }
        
        public IEnumerable<ulong> Maps
        {
            get { return _repository.Maps; }
        }

        public IMapProcessor Write(Stream source)
        {
            return _repository.Write(source);
        }

        public IMapProcessor Read(ulong mapId, Stream target)
        {
           return _repository.Read(mapId, target);
        }

        public IMapProcessor Delete(ulong mapId)
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
