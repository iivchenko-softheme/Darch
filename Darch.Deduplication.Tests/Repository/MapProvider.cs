// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Darch.Deduplication.Storages;
using Shogun.Patterns.Repositories;

namespace Darch.Deduplication.Tests.Repository
{
    public sealed class MapProvider : Darch.Deduplication.Storages.MapProvider
    {
        private const string Extension = ".map";

        private readonly IStreamMapper<MapRecord> _mapMapper;
        private readonly string _workDirectory;
        private readonly int _itemSize;

        public MapProvider(IStreamMapper<MapRecord> mapMapper, int itemSize, string workDirectory)
        {
            _mapMapper = mapMapper;
            _workDirectory = workDirectory;
            _itemSize = itemSize;
        }

        public override IEnumerable<ulong> Ids
        {
            get
            {
                return Directory
                            .GetFiles(_workDirectory, "*.map")
                            .Select(x => new FileInfo(x))
                            .Select(x => ulong.Parse(x.Name.Split('.')[0]));
            }
        }

        protected override IRepository<MapRecord, ulong> CreateRepository(ulong mapId)
        {
            var stream = !Ids.Contains(mapId) ? CreateMap(mapId) : Open(mapId);

            return new StreamRepository<MapRecord>(stream, _mapMapper, _itemSize);
        }

        private Stream CreateMap(ulong mapId)
        {
            return File.Create(Path.Combine(_workDirectory, mapId + Extension));
        }

        private Stream Open(ulong mapId)
        {
            return File.Open(Path.Combine(_workDirectory, mapId + Extension), FileMode.Open);
        }
    }
}
