// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.IO;
using Deduplication.Storages;

namespace Deduplication.Maps
{
    public class MapProcessorFactory : IMapProcessorFactory
    {
        private readonly IStorage _storage;

        public MapProcessorFactory(IStorage storage)
        {
            _storage = storage;
        }

        public IMapProcessor CreateReadProcessor(ulong mapId, Stream target)
        {
            return new ReadProcessor(mapId, target, _storage);
        }

        public IMapProcessor CreateWriteProcessor(ulong mapId, int blockSize, Stream source)
        {
            return new WriteProcessor(mapId, blockSize, source, _storage);
        }

        public IMapProcessor CreateDeleteProcessor(ulong mapId)
        {
            return new DeleteProcessor(mapId, _storage);
        }
    }
}
