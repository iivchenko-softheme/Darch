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

namespace Deduplication.Tests.Repository
{
    public class MapStreamMapper : IStreamMapper<MapRecord>
    {
        public const int BufferSize = sizeof(ulong) * 3; // MapRecord contains 3 ulong Property which should be saved in a repository

        public byte[] Convert(MapRecord item)
        {
            var buffer = new List<byte>();

            buffer.AddRange(BitConverter.GetBytes(item.MapId));
            buffer.AddRange(BitConverter.GetBytes(item.BlockId));
            buffer.AddRange(BitConverter.GetBytes(item.RecordIndex));

            ValidateSize(buffer.Count);

            return buffer.ToArray();
        }

        public MapRecord Convert(byte[] buffer)
        {
            ValidateSize(buffer.Length);

            return new MapRecord(
                BitConverter.ToUInt64(buffer.Take(8).ToArray(), 0),
                BitConverter.ToUInt64(buffer.Skip(8).Take(8).ToArray(), 0),
                BitConverter.ToUInt64(buffer.Skip(16).ToArray(), 0));
        }

        private static void ValidateSize(int size)
        {
            if (size != BufferSize)
            {
                throw new InvalidOperationException(string.Format("Specified buffer size '{0}' doesn't correspond to an expected size '{1}'.", size, BufferSize));
            }
        }
    }
}
