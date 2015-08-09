// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.IO;
using Darch.Deduplication.Storages;
using Shogun.Utility.Jobs;

namespace Darch.Deduplication.Maps
{
    public class MapProcessorFactory : IMapProcessorFactory
    {
        private readonly IStorage _storage;

        public MapProcessorFactory(IStorage storage)
        {
            _storage = storage;
        }

        public IJob CreateReadProcessor(ulong mapId, Stream target)
        {
            return new ReadProcessor(mapId, target, _storage);
        }

        public IJob CreateWriteProcessor(ulong mapId, int blockSize, Stream source)
        {
            return new WriteProcessor(mapId, blockSize, source, _storage);
        }

        public IJob CreateDeleteProcessor(ulong mapId)
        {
            return new DeleteProcessor(mapId, _storage);
        }
    }
}
