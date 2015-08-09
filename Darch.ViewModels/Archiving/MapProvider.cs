// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Packaging;
using System.Linq;
using Darch.Deduplication.Storages;
using Shogun.Patterns.Repositories;

namespace Darch.ViewModels.Archiving
{
    public sealed class MapProvider : Deduplication.Storages.MapProvider
    {
        private readonly Package _package;

        public MapProvider(Package package)
        {
            _package = package;
            _package.GetParts();
        }

        public override IEnumerable<ulong> Ids
        {
            get
            {
                return _package
                            .GetParts()
                            .Where(x => x.Uri.OriginalString.StartsWith("/maps", StringComparison.OrdinalIgnoreCase))
                            .Select(x => ulong.Parse(x.Uri.OriginalString.Split('/')[2], CultureInfo.InvariantCulture));
            }
        }

        protected override IRepository<MapRecord, ulong> CreateRepository(ulong mapId)
        {
            var uri = new Uri("/Maps/" + mapId, UriKind.Relative);

            var part = _package
                .GetParts()
                .FirstOrDefault(x => x.Uri.OriginalString.Equals(uri.OriginalString, StringComparison.OrdinalIgnoreCase));

            return part == null 
                ? new StreamRepository<MapRecord>(_package.CreatePart(uri, string.Empty).GetStream(), new MapStreamMapper(), MapStreamMapper.BufferSize)
                : new StreamRepository<MapRecord>(_package.GetPart(uri).GetStream(), new MapStreamMapper(), MapStreamMapper.BufferSize);
        }
    }
}
