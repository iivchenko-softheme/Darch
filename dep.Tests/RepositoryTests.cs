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

        private int _seed;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Seed: {0}", _seed);
        }

        [Test]
        public void WriteRead_Test()
        {
            var dataGenerator = new DataGenerator(_seed);
            var sourceStream = dataGenerator.GenerateData(SourceStreamSize);
            var targetStream = new MemoryStream();

            using (var repo = new ManagedRepository(BlockSize, ChecksumSize))
            {
                using (var map = repo.Write(sourceStream))
                using (new MapMonitor(map))
                {
                    map.Start();
                    map.Wait();
                }
                
                Console.WriteLine();

                using (var map = repo.Read(0, targetStream))
                using (new MapMonitor(map))
                {
                    map.Start();
                    map.Wait();
                }
            }

            Assert.AreEqual(sourceStream, targetStream);
        }
    }
}
