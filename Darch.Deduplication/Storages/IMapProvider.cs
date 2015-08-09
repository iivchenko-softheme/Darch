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
    public interface IMapProvider : IDisposable
    {
        IEnumerable<ulong> Ids { get; }

        IRepository<MapRecord, ulong> this[ulong mapId] { get; }
    }
}
