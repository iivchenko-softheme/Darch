// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.IO;
using System.Security.Cryptography;

namespace Deduplication.Tests.Data
{
    public sealed class DataManager
    {
        private const int MaxIntBufferSize = int.MaxValue;
        private readonly int _seed;

        public DataManager(int seed)
        {
            _seed = seed;
        }

        public MemoryStream GenerateData(int streamSize)
        {
            var random = new Random(_seed);
            var buffer = new byte[random.Next(streamSize)];

            random.NextBytes(buffer);

            return new MemoryStream(buffer);
        }

        public FileInfo GenerateFile(int fileSize)
        {
            byte[] hash;
            var random = new Random(_seed);

            var path = Path.GetTempFileName();
            var bytesToWrite = fileSize;

            using (var file = File.Create(path))
            {
                while (bytesToWrite > 0)
                {
                    byte[] buffer;

                    if (bytesToWrite > MaxIntBufferSize)
                    {
                        buffer = new byte[random.Next(MaxIntBufferSize)];
                        bytesToWrite -= MaxIntBufferSize;
                    }
                    else
                    {
                        buffer = new byte[random.Next(bytesToWrite)];
                        bytesToWrite = 0;
                    }

                    random.NextBytes(buffer);

                    file.Write(buffer, 0, buffer.Length);
                }
            }

            using (var file = File.OpenRead(path))
            using (var md5 = new MD5Cng())
            {
                hash = md5.ComputeHash(file);
            }

            return new FileInfo
                   {
                       Path = path,
                       Hash = hash,
                       Size = fileSize
                   };
        }

        public byte[] CalculateHash(string filePath)
        {
            using (var file = File.OpenRead(filePath))
            using (var md5 = new MD5Cng())
            {
                return md5.ComputeHash(file);
            }
        }
    }
}
