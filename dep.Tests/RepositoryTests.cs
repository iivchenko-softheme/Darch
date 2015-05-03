// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.IO;
using System.Linq;
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

        [Test]
        [Description("Add some maps. Delete on of them. Check if deleted map is no longer available.")]
        public void Delete_Test()
        {
            var dataGenerator = new DataGenerator(_seed);
            var sourceStream = dataGenerator.GenerateData(SourceStreamSize);

            using (var repo = new ManagedRepository(BlockSize, ChecksumSize))
            {
                for (var i = 0; i < 3; i++)
                {
                    sourceStream.Seek(0, SeekOrigin.Begin);
                    using (var map = repo.Write(sourceStream))
                    using (new MapMonitor(map))
                    {
                        map.Start();
                        map.Wait();
                    }

                    Console.WriteLine();
                }

                Assert.Contains(0, repo.Maps.ToList());
                Assert.Contains(1, repo.Maps.ToList());
                Assert.Contains(2, repo.Maps.ToList());

                using (var map = repo.Delete(1))
                using (new MapMonitor(map))
                {
                    map.Start();
                    map.Wait();
                }

                Assert.Contains(0, repo.Maps.ToList());
                Assert.Contains(2, repo.Maps.ToList());

                Assert.IsFalse(repo.Maps.Any(x => x == 1), "Map with id 1 should be deleted!");
            }
        }

        [Test]
        [Description("Generate a set of data and add them two times. After first addition Data and Metadat streams should not grow.")]
        public void Deduplication_Test()
        {
            // Generate test data
            var dataGenerator = new DataGenerator(_seed);
            var sourceStream = dataGenerator.GenerateData(SourceStreamSize);

            using (var repo = new ManagedRepository(BlockSize, ChecksumSize))
            { 
                sourceStream.Seek(0, SeekOrigin.Begin);
                using (var map = repo.Write(sourceStream))
                using (new MapMonitor(map))
                {
                    map.Start();
                    map.Wait();
                }

                Console.WriteLine();

                // Calculate expecdet values
                var expectedDataSize = repo.DataStream.Length;
                var expectedMetadataSize = repo.MetadataStream.Length;

                sourceStream.Seek(0, SeekOrigin.Begin);
                using (var map = repo.Write(sourceStream))
                using (new MapMonitor(map))
                {
                    map.Start();
                    map.Wait();
                }

                // Second addition of the same data should not change expected values
                var actualdDataSize = repo.DataStream.Length;
                var actualMetadataSize = repo.MetadataStream.Length;
                
                Assert.AreEqual(expectedMetadataSize, actualMetadataSize, "Metadata");
                Assert.AreEqual(expectedDataSize, actualdDataSize, "Data");
            }
        }
    }
}
