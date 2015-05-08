// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;

namespace Darch.Deduplication.Storages
{
    public interface IStorage : IDisposable
    {
        IEnumerable<ulong> MapIds { get; }

        IEnumerable<ulong> ReadMap(ulong mapId);

        void AddBlockItem(ulong mapId, byte[] block, int realSize);

        byte[] ReadBlockItem(ulong mapId, ulong blockIndex);

        void DeleteBlockItem(ulong mapId, ulong blockIndex);
    }
}
