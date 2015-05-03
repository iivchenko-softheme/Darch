// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Linq;
using Deduplication.Storages;
using RepositoryLib;

namespace Deduplication.Tests
{
    public sealed class MetadataStreamMapper : IStreamMapper<MetadataItem>
    {
        private const int ULongSize = sizeof(ulong);

        public MetadataStreamMapper(int checksumSize)
        {
            ChecksumSize = checksumSize;
            BufferSize = (ULongSize * 3) + ChecksumSize;
        }

        public int BufferSize { get; private set; }

        public int ChecksumSize { get; private set; }

        public byte[] Convert(MetadataItem item)
        {
            var buffer = new List<byte>();

            buffer.AddRange(BitConverter.GetBytes(item.DataId));
            buffer.AddRange(item.Checksum);
            buffer.AddRange(BitConverter.GetBytes(item.Size));
            buffer.AddRange(BitConverter.GetBytes(item.References));

            ValidateSize(buffer.Count);

            return buffer.ToArray();
        }

        public MetadataItem Convert(byte[] buffer)
        {
            ValidateSize(buffer.Length);

            return new MetadataItem
            {
                DataId = BitConverter.ToUInt64(buffer.Take(ULongSize).ToArray(), 0),
                Checksum = buffer.Skip(ULongSize).Take(ChecksumSize).ToArray(),
                Size = BitConverter.ToUInt64(buffer.Skip(ULongSize + ChecksumSize).Take(ULongSize).ToArray(), 0),
                References = BitConverter.ToUInt64(buffer.Skip((ULongSize * 2) + ChecksumSize).Take(ULongSize).ToArray(), 0),
            };
        }

        private void ValidateSize(int size)
        {
            if (size != BufferSize)
            {
                throw new InvalidOperationException(string.Format("Specified buffer size '{0}' doesn't correspond to an expected size '{1}'.", size, BufferSize));
            }
        }
    }
}
