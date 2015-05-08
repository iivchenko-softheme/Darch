// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Shogun.Patterns.Repositories
{
    /// <summary>
    /// The repository based on .net Stream. The stream is a storage used by the Repository.
    /// </summary>
    /// <typeparam name="TItem">Type that should be stored in the storage.</typeparam>
    public sealed class StreamRepository<TItem> : IRepository<TItem, ulong>
    {
        private const int IdSize = sizeof(int);

        private readonly Stream _stream;
        private readonly IStreamMapper<TItem> _streamMapper;
        private readonly int _itemSize;

        // contains pair of item ID and item INDEX in the repository
        private readonly Lazy<IDictionary<ulong, long>> _cache;
        
        /// <summary>
        /// Initializes a new instance of the StreamRepository class.
        /// </summary>
        /// <param name="stream">A stream which will be used as a storage for items.</param>
        /// <param name="streamMapper">Provides conversation algorithms from <see cref="TItem"/> to a bytes and from bytes to a <see cref="TItem"/> to write and read <see cref="TItem"/> from <param name="stream"/>.</param>
        /// <param name="itemSize">Size of each <see cref="TItem"/> in bytes.</param>
        public StreamRepository(
            Stream stream,
            IStreamMapper<TItem> streamMapper,
            int itemSize)
        {
            _stream = stream;
            _streamMapper = streamMapper;
            _itemSize = itemSize;

            _cache = new Lazy<IDictionary<ulong, long>>(() =>
                                                      {
                                                          var cache = new Dictionary<ulong, long>();
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

        public IEnumerable<RepositoryItem<ulong, TItem>> All
        {
            get
            {
                return _cache.Value.Keys.Select(id => new RepositoryItem<ulong, TItem>(id, ReadItem(GetItemIndex(id))));
            }
        }

        public TItem GetById(ulong id)
        {
            ThrowIfItemNotExists(id);

            return ReadItem(GetItemIndex(id));
        }

        public ulong Add(TItem item)
        {
            var newId = _cache.Value.Count == 0 ? 1 : _cache.Value.Keys.Max() + 1;
            var newIndex = _stream.Length;

            WriteIdAndItem(newIndex, newId, item);

            _cache.Value.Add(newId, newIndex);

            return newId;
        }

        public void Delete(ulong id)
        {
            ThrowIfItemNotExists(id);

            var indexToDelete = _cache.Value[id];
            var indexToMove = _stream.Length - (IdSize + _itemSize); // Take last item in the repository

            _cache.Value.Remove(id);

            // In case if we delete the only one element or last element
            if (indexToDelete == indexToMove)
            {
                _stream.SetLength(_stream.Length - (IdSize + _itemSize));
                return;
            }

            var idToMove = ReadId(indexToMove);
            var itemToMove = ReadItem(indexToMove + IdSize);

            WriteIdAndItem(indexToDelete, idToMove, itemToMove); // Replace item we want to delete with last repository item.

            _cache.Value[idToMove] = indexToDelete;

            _stream.SetLength(_stream.Length - (IdSize + _itemSize));
        }

        public void Update(ulong id, TItem item)
        {
            ThrowIfItemNotExists(id);

            WriteItem(GetItemIndex(id), item);
        }

        public void Dispose()
        {
            // TODO: !!! Implement
        }

        private static void ValidateItem(int actualSize, int expectedSize)
        {
            if (actualSize != expectedSize)
            {
                var msg = string.Format(
                 CultureInfo.InvariantCulture,
                 "Can't add new item to the repository. Stream repository item is corrupted! Future item size is '{0}'. Expected item size '{1}'.",
                 actualSize,
                 expectedSize);

                throw new ItemCorruptedException(msg);
            }
        }

        private void ThrowIfItemNotExists(ulong id)
        {
            if (!_cache.Value.ContainsKey(id))
            {
                var msg = string.Format(
                    CultureInfo.InvariantCulture, 
                    "The repository doesn't contain item with id {0}", 
                    id);

                throw new MissingItemException(msg);
            }
        }

        private TItem ReadItem(long index)
        {
            var buffer = new byte[_itemSize];

            _stream.Seek(index, SeekOrigin.Begin);
            var bytesRead = _stream.Read(buffer, 0, _itemSize);

            if (bytesRead != _itemSize)
            {
                var msg = string.Format(
                    CultureInfo.InvariantCulture,
                    "Stream repository item is corrupted! Read item at index '{0}'. Expected item size '{1}' but was read '{2}' size.",
                    index,
                    _itemSize,
                    bytesRead);

                throw new ItemCorruptedException(msg);
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
                var msg = string.Format(
                    CultureInfo.InvariantCulture,
                    "Stream repository item is corrupted! Read item ID at index '{0}'. Expected ID size '{1}' but was read '{2}' size.",
                    index,
                    IdSize,
                    bytesRead);

                throw new ItemCorruptedException(msg);
            }

            return BitConverter.ToUInt32(buffer, 0);
        }

        private void WriteIdAndItem(long index, ulong id, TItem item)
        {
            var idBuffer = BitConverter.GetBytes(id);
            var itemBuffer = _streamMapper.Convert(item);

            ValidateItem(itemBuffer.Length, _itemSize);
            
            _stream.Seek(index, SeekOrigin.Begin);

            _stream.Write(idBuffer, 0, IdSize);
            _stream.Write(itemBuffer, 0, _itemSize);
        }

        private void WriteItem(long index, TItem item)
        {
            var buffer = _streamMapper.Convert(item);

            ValidateItem(buffer.Length, _itemSize);
            
            _stream.Seek(index, SeekOrigin.Begin);
            _stream.Write(buffer, 0, _itemSize);
        }

        private long GetItemIndex(ulong id)
        {
            return _cache.Value[id] + IdSize;
        }
    }
}
