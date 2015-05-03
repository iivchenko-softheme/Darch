// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Collections.Generic;
using System.IO;
using Deduplication.Maps;
using Deduplication.Storages;
using RepositoryLib;

namespace Deduplication.Tests.Repository
{
    public sealed class ManagedRepository : IRepository
    {
        private readonly IRepository _repository;

        public ManagedRepository(int blockSize, int checksumSize)
        {
            var hash = new MD5Hash();

            MapStream = new MemoryStream();
            MetadataStream = new MemoryStream();
            DataStream = new MemoryStream();

            var mapStreamMapper = new MapStreamMapper();
            var mapRepository = new StreamRepository<MapRecord>(MapStream, mapStreamMapper, MapStreamMapper.BufferSize);
            
            var metadataStreamMapper = new MetadataStreamMapper(checksumSize);
            var metadataRepository = new StreamRepository<MetadataItem>(MetadataStream, metadataStreamMapper, metadataStreamMapper.BufferSize);
            
            var dataStreamMapper = new DataStreamMapper(blockSize);
            var dataRepository = new StreamRepository<byte[]>(DataStream, dataStreamMapper, blockSize);

            var storage = new Storage(hash, mapRepository, metadataRepository, dataRepository);
            var mapProcessorFactory = new MapProcessorFactory(storage);

            _repository = new Deduplication.Repository(storage, mapProcessorFactory, blockSize);
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
        }
    }
}
