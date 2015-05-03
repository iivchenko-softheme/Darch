// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.IO;
using Deduplication.Maps;
using Deduplication.Storages;
using NUnit.Framework;
using RepositoryLib;

namespace Deduplication.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        private const int Md5ChecksumSize = 16;
        private const int BlockSize = 8 * 1024;
        private const int ChecksumSize = Md5ChecksumSize;

        private const int SourceStreamSize = 10 * 1024 * 1024;

        private Random _random;

        [TestFixtureSetUp]
        public void Initialize()
        {
            var seed = (int)DateTime.Now.Ticks;

            _random = new Random(seed);

            Console.WriteLine("Seed: {0}", seed);
        }

        [Test]
        public void WriteRead_Test()
        {
            var sourceStream = GenerageData();
            var targetStream = new MemoryStream();

            using (var repo = CreateRepository(BlockSize, ChecksumSize))
            {
                using (var writeMap = repo.Write(sourceStream))
                {
                    Console.WriteLine("Start write to the repo.");
                    Console.WriteLine("Write Map Id: {0}", writeMap.Id);

                    ulong prev = 0;
                    writeMap.ProgressChanged += (sender, args) =>
                    {
                        var newV = (args.Progress.WorkDone * 100) / args.Progress.WorkTotal;
                        if (newV > prev + 5)
                        {
                            Console.WriteLine("Total: {0}%; Done: {1}", 100, newV);
                            
                            prev = newV;
                        }
                    };

                    writeMap.StatusChanged += (sender, args) =>
                    {
                        Console.WriteLine("New status: {0}", args.Status);
                    };

                    writeMap.Start();
                    writeMap.Wait();
                }

                using (var readMap = repo.Read(0, targetStream))
                {
                    Console.WriteLine("Start read from the repo.");
                    Console.WriteLine("Read Map Id: {0}", readMap.Id);

                    readMap.ProgressChanged += (sender, args) =>
                    {
                        Console.WriteLine("Total: {0}; Done: {1}", args.Progress.WorkTotal, args.Progress.WorkDone);
                    };

                    readMap.StatusChanged += (sender, args) =>
                    {
                        Console.WriteLine("New status: {0}", args.Status);
                    };

                    readMap.Start();
                    readMap.Wait();
                }
            }

            Assert.AreEqual(sourceStream, targetStream);
        }

        private static IRepository CreateRepository(int blockSize, int checksumSize)
        {
            var hash = new MD5Hash();

            var mapStream = new MemoryStream();
            var mapStreamMapper = new MapStreamMapper();
            var mapRepository = new StreamRepository<MapRecord>(mapStream, mapStreamMapper, MapStreamMapper.BufferSize);

            var metadataStream = new MemoryStream();
            var metadataStreamMapper = new MetadataStreamMapper(checksumSize);
            var metadataRepository = new StreamRepository<MetadataItem>(metadataStream, metadataStreamMapper, metadataStreamMapper.BufferSize);

            var dataStream = new MemoryStream();
            var dataStreamMapper = new DataStreamMapper(blockSize);
            var dataRepository = new StreamRepository<byte[]>(dataStream, dataStreamMapper, blockSize);

            var storage = new Storage(hash, mapRepository, metadataRepository, dataRepository);
            var mapProcessorFactory = new MapProcessorFactory(storage);

            return new Repository(storage, mapProcessorFactory, blockSize);
        }

        private MemoryStream GenerageData()
        {
            var buffer = new byte[_random.Next(SourceStreamSize)];

            _random.NextBytes(buffer);

            return new MemoryStream(buffer);
        }
    }
}
