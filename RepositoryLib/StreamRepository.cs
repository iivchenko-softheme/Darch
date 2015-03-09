// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RepositoryLib
{
    // TODO: Create documentation
    // TODO: Implement integratioon tests
    // TODO: Implement performance tests
    // TODO: Implement item size validation
    // TODO: Check algorithms
    public sealed class StreamRepository<TItem> : IRepository<TItem, uint>
    {
        private const int IdSize = sizeof(int);

        private readonly Stream _stream;
        private readonly IStreamMapper<TItem> _streamMapper;
        private readonly int _itemSize;

        private readonly Lazy<IDictionary<uint, long>> _cache;
        
        public StreamRepository(
            Stream stream,
            IStreamMapper<TItem> streamMapper,
            int itemSize)
        {
            _stream = stream;
            _streamMapper = streamMapper;
            _itemSize = itemSize;

            _cache = new Lazy<IDictionary<uint, long>>(() =>
                                                      {
                                                          var cache = new Dictionary<uint, long>();
                                                          long index = 0;

                                                          while (index < _stream.Length)
                                                          {
                                                              var id = ReadId(index);

                                                              cache.Add(id, index);

                                                              index += IdSize + _itemSize;
                                                          }

                                                          return cache;
                                                      });
        }
        
        public uint Count
        {
            get { return (uint)_stream.Length / (uint)(IdSize + _itemSize); }
        }

        public IEnumerable<TItem> All
        {
            get
            {
                // TODO: Check speed for the algorithm
                return _cache.Value.Keys.Select(id => ReadItem(GetItemIndex(id)));
            }
        }

        public TItem GetById(uint id)
        {
            ThrowIfItemNotExists(id);

            return ReadItem(GetItemIndex(id));
        }

        public uint Add(TItem item)
        {
            var newId = _cache.Value.Keys.Max() + 1;
            var newIndex = _stream.Length;

            WriteIdAndItem(newIndex, newId, item);

            _cache.Value.Add(newId, newIndex);

            return newId;
        }

        public void Delete(uint id)
        {
            ThrowIfItemNotExists(id);

            var indexToDelete = _cache.Value[id];
            var indexToMove = _stream.Length - (IdSize + _itemSize);

            _cache.Value.Remove(id);

            // In case if we delete only one element
            if (indexToDelete == indexToMove)
            {
                _stream.SetLength(_stream.Length - (IdSize + _itemSize));
                return;
            }

            var idToMove = ReadId(indexToMove);
            var itemToMove = ReadItem(indexToMove + IdSize);

            WriteIdAndItem(indexToDelete, idToMove, itemToMove);

            _stream.SetLength(_stream.Length - (IdSize + _itemSize));
        }

        public void Update(uint id, TItem item)
        {
            ThrowIfItemNotExists(id);

            WriteItem(GetItemIndex(id), item);
        }

        private void ThrowIfItemNotExists(uint id)
        {
            if (!_cache.Value.ContainsKey(id))
            {
                // TODO: Implement
                throw new NotImplementedException();
            }
        }

        private TItem ReadItem(long index)
        {
            var buffer = new byte[_itemSize];

            _stream.Seek(index, SeekOrigin.Begin);
            var bytesRead = _stream.Read(buffer, 0, _itemSize);

            if (bytesRead != _itemSize)
            {
                // TODO: throw
            }

            return _streamMapper.Convert(buffer);
        }

        private uint ReadId(long index)
        {
            var buffer = new byte[IdSize];

            _stream.Seek(index, SeekOrigin.Begin);
            var bytesRead = _stream.Read(buffer, 0, IdSize);

            if (bytesRead != IdSize)
            {
                // TODO: throw
            }

            return BitConverter.ToUInt32(buffer, 0);
        }

        private void WriteIdAndItem(long index, uint id, TItem item)
        {
            var buffer = BitConverter.GetBytes(id);

            _stream.Seek(index, SeekOrigin.Begin);

            _stream.Write(buffer, 0, _itemSize);

            buffer = _streamMapper.Convert(item);

            _stream.Write(buffer, 0, _itemSize);
        }

        private void WriteItem(long index, TItem item)
        {
            var buffer = _streamMapper.Convert(item);

            _stream.Seek(index, SeekOrigin.Begin);
            _stream.Write(buffer, 0, _itemSize);
        }

        private long GetItemIndex(uint id)
        {
            return _cache.Value[id] + IdSize;
        }
    }
}
