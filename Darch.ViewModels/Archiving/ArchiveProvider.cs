// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using Darch.Deduplication;
using Darch.Deduplication.Maps;
using Darch.Deduplication.Storages;
using Shogun.Patterns.Repositories;

// TODO: Unify priciples of Creating and Opening of Parts. Make only one point of access to the TYPES of PARTS.
namespace Darch.ViewModels.Archiving
{
    public class ArchiveProvider : IArchiveProvider
    {
        private const int Md5ChecksumSize = 16;
        private const int BlockSize = 8 * 1024;
        private const int ChecksumSize = Md5ChecksumSize;

        private readonly IHash _hash;

        public ArchiveProvider(IHash hash)
        {
            _hash = hash;
        }

        public Archive Open(string path)
        {
            var package = Package.Open(path, FileMode.Open);
            var metadata = package.GetPart(new Uri("/Metadata.xml", UriKind.Relative)).GetStream();
            var mapRepository = OpenRepository(package, "/Map.dat", new MapStreamMapper(), MapStreamMapper.BufferSize);
            var metadataStreamMapper = new MetadataStreamMapper(ChecksumSize);
            var metadataRepository = OpenRepository(package, "/Metadata.dat", metadataStreamMapper, metadataStreamMapper.BufferSize);
            var dataRepository = OpenRepository(package, "/Data.dat", new DataStreamMapper(BlockSize), BlockSize);
            var storage = new Storage(_hash, mapRepository, metadataRepository, dataRepository);
            var mapProcessor = new MapProcessorFactory(storage);
            var repository = new Repository(mapProcessor, storage.MapIds.ToList(), BlockSize);
            
            return new Archive(package, repository, metadata);
        }

        public Archive Create(string path)
        {
            var package = Package.Open(path, FileMode.Create);
            var metadata = package.CreatePart(new Uri("/Metadata.xml", UriKind.Relative), System.Net.Mime.MediaTypeNames.Text.Xml).GetStream();
            var mapRepository = CreateRepository(package, "/Map.dat", new MapStreamMapper(), MapStreamMapper.BufferSize);
            var metadataStreamMapper = new MetadataStreamMapper(ChecksumSize);
            var metadataRepository = CreateRepository(package, "/Metadata.dat", metadataStreamMapper, metadataStreamMapper.BufferSize);
            var dataRepository = CreateRepository(package, "/Data.dat", new DataStreamMapper(BlockSize), BlockSize);
            var storage = new Storage(_hash, mapRepository, metadataRepository, dataRepository);
            var mapProcessor = new MapProcessorFactory(storage);
            var repository = new Repository(mapProcessor, storage.MapIds.ToList(), BlockSize);

            package.Flush();

            return new Archive(package, repository, metadata);
        }

        private static IRepository<TItem, ulong> CreateRepository<TItem>(Package package, string path, IStreamMapper<TItem> streamMapper, int itemSize)
        {
            var uri = new Uri(path, UriKind.Relative);
            return new StreamRepository<TItem>(package.CreatePart(uri, string.Empty).GetStream(), streamMapper, itemSize);
        }

        private static IRepository<TItem, ulong> OpenRepository<TItem>(Package package, string path, IStreamMapper<TItem> streamMapper, int itemSize)
        {
            var uri = new Uri(path, UriKind.Relative);
            return new StreamRepository<TItem>(package.GetPart(uri).GetStream(), streamMapper, itemSize);
        }
    }
}
