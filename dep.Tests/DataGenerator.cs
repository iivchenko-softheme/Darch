// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.IO;

namespace Deduplication.Tests
{
    public sealed class DataGenerator
    {
        private readonly Random _random;

        public DataGenerator(int seed)
        {
            _random = new Random(seed);
        }

        public MemoryStream GenerateData(int streamSize)
        {
            var buffer = new byte[_random.Next(streamSize)];

            _random.NextBytes(buffer);

            return new MemoryStream(buffer);
        }
    }
}
