// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Darch.Deduplication.Tests.Data;
using Darch.Deduplication.Tests.Repository;
using NUnit.Framework;
using FileInfo = Darch.Deduplication.Tests.Data.FileInfo;

namespace Darch.Deduplication.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        private const int Md5ChecksumSize = 16;
        private const int BlockSize = 8 * 1024;
        private const int ChecksumSize = Md5ChecksumSize;

        private const int SourceStreamSize = 512 * 1024 * 1024;

        private int _seed;
        private string _workDir;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Seed: {0}", _seed);
        }

        [SetUp]
        public void Setup()
        {
            _workDir = Path.Combine(Path.GetTempPath(), "Performance_Test\\");

            if (Directory.Exists(_workDir))
            {
                Directory.Delete(_workDir, true);
            }

            Directory.CreateDirectory(_workDir);
        }

        [TearDown]
        public void Teardown()
        {
            _workDir = Path.Combine(Path.GetTempPath(), "Performance_Test\\");

            if (Directory.Exists(_workDir))
            {
                Directory.Delete(_workDir, true);
            }
        }

        [Test]
        public void WriteRead_Test()
        {
            var dataGenerator = new DataManager(_seed);
            var sourceStream = dataGenerator.GenerateData(SourceStreamSize);
            var targetStream = new MemoryStream();

            using (var repo = new ManagedRepository(BlockSize, ChecksumSize, _workDir))
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
            var dataGenerator = new DataManager(_seed);
            var sourceStream = dataGenerator.GenerateData(10);

            using (var repo = new ManagedRepository(BlockSize, ChecksumSize, _workDir))
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
            var dataGenerator = new DataManager(_seed);
            var sourceStream = dataGenerator.GenerateData(SourceStreamSize);
            
            using (var repo = new ManagedRepository(BlockSize, ChecksumSize, _workDir))
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

        [Test]
        [Explicit]
        [Description("Generate several files. Write them to a repository. Restore them and check integrity. Also measure speed of Write, Read and Delete.")]
        public void Performance_Test()
        {
            const int FilesCount = 1;
            const int FileSize = 512 * 1024 * 1024;

            Func<string> getFilePath = () => Path.Combine(_workDir, Path.GetRandomFileName());

            var sourceFiles = new List<FileInfo>(FilesCount);
            var targetFiles = new List<FileInfo>(FilesCount);
            var dataManager = new DataManager(_seed);

            Action<FileInfo, IRepository> write = (fileInfo, repo) =>
                                                              {
                                                                  using (var file = File.Open(fileInfo.Path, FileMode.Open))
                                                                  using (var map = repo.Write(file))
                                                                  using (new MapMonitor(map))
                                                                  {
                                                                      map.Start();
                                                                      map.Wait();

                                                                      fileInfo.MapId = map.Id;
                                                                  }
                                                              };

            Action<FileInfo, IList<FileInfo>, IRepository> read = (fileInfo, files, repo) =>
                                                             {
                                                                 var path = getFilePath();

                                                                 using (var file = File.Create(path))
                                                                 using (var map = repo.Read(fileInfo.MapId, file))
                                                                 using (new MapMonitor(map))
                                                                 {
                                                                     map.Start();
                                                                     map.Wait();
                                                                 }

                                                                 var targetFile = new FileInfo
                                                                                  {
                                                                                      Hash = dataManager.CalculateHash(path),
                                                                                      Path = path,
                                                                                      MapId = fileInfo.MapId,
                                                                                      Size = 0 // calculate
                                                                                  };

                                                                 targetFiles.Add(targetFile);
                                                             };

            using (var repo = new ManagedRepository(BlockSize, ChecksumSize, _workDir))
            {
                Console.WriteLine("=== Generating files (count: {0}) ===", FilesCount);
                Parallel.For(0, FilesCount, i => sourceFiles.Add(dataManager.GenerateFile(FileSize)));
                Console.WriteLine();

                Console.WriteLine("=== Writing files to the repo ===");
                Parallel.ForEach(sourceFiles, file => write(file, repo));
                Console.WriteLine();

                Console.WriteLine("=== Reading files from the repo ===");
                Parallel.ForEach(sourceFiles, mapId => read(mapId, targetFiles, repo));
                Console.WriteLine();
                
                foreach (var source in sourceFiles)
                {
                    var target = targetFiles.Single(x => x.MapId == source.MapId);

                    Assert.AreEqual(source.Hash, target.Hash);
                }
            }
        }
    }
}
