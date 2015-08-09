// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using Shogun.Patterns.Repositories;

namespace Darch.Deduplication.Storages
{
    public abstract class MapProvider : IMapProvider
    {
        private readonly IDictionary<ulong, IRepository<MapRecord, ulong>> _maps;

        protected MapProvider()
        {
            _maps = new Dictionary<ulong, IRepository<MapRecord, ulong>>();
        }

        ~MapProvider()
        {
            Dispose(false);
        }

        public abstract IEnumerable<ulong> Ids { get; }

        public IRepository<MapRecord, ulong> this[ulong mapId]
        {
            get
            {
                IRepository<MapRecord, ulong> map = null;
                if (!_maps.TryGetValue(mapId, out map))
                {
                    map = CreateRepository(mapId);
                    _maps.Add(mapId, map);
                }

                return map;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected abstract IRepository<MapRecord, ulong> CreateRepository(ulong mapId);

        protected virtual void Dispose(bool disposing)
        {
            foreach (var repository in _maps.Values)
            {
                repository.Dispose();
            }

            _maps.Clear();
        }
    }
}
