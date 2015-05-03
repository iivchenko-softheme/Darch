// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.IO;
using Deduplication.Tests.Repository;
using NUnit.Framework;

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

            using (var repo = new ManagedRepository(BlockSize, ChecksumSize))
            {
                using (var writeMap = repo.Write(sourceStream))
                {
                    Console.WriteLine("Start write to the repo.");
                    Console.WriteLine("Write Map Id: {0}", writeMap.Id);

                    var monitor = new ProgressMonitor("Write", 5);

                    writeMap.ProgressChanged += monitor.OnProgress;
                    writeMap.StatusChanged += monitor.OnStatus;

                    writeMap.Start();
                    writeMap.Wait();
                }

                using (var readMap = repo.Read(0, targetStream))
                {
                    Console.WriteLine("Start read from the repo.");
                    Console.WriteLine("Read Map Id: {0}", readMap.Id);

                    var monitor = new ProgressMonitor("Read", 5);
                    
                    readMap.ProgressChanged += monitor.OnProgress;
                    readMap.StatusChanged += monitor.OnStatus;

                    readMap.Start();
                    readMap.Wait();
                }
            }

            Assert.AreEqual(sourceStream, targetStream);
        }

        private MemoryStream GenerageData()
        {
            var buffer = new byte[_random.Next(SourceStreamSize)];

            _random.NextBytes(buffer);

            return new MemoryStream(buffer);
        }
    }
}
