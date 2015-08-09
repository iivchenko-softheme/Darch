// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shogun.Patterns.Repositories;
using Shogun.Utility.Logging;

namespace Shogun.Patterns.Tests.Performance.Repositories
{
    [TestFixture]
    public class StreamRepositoryTests
    {
        private Configurator _configurator;
        private Random _random;

        [TestFixtureSetUp]
        public void Initialize()
        {
            _configurator = new Configurator
            {
                Seed = (int)DateTime.Now.Ticks,
                Storage = Configurator.StorageType.File,
                LogLevel = LogLevel.Info,
                LogFlushPolicy = FlushPolicy.Buffered,
                BlockSize = 8 * 1024 // 8 kb
            };

            Console.WriteLine(_configurator.ToString());

            _random = new Random(_configurator.Seed);
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            _configurator.Clear();
        }

        #region Performance tests

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(20000)]
        [TestCase(30000)]
        [TestCase(40000)]
        [TestCase(50000)]
        [TestCase(60000)]
        [TestCase(100000)]
        [Description("Just add so many items as it possible and calculate the speed.")]
        [Explicit]
        public void PerformanceTest_Add(int count)
        {
            var repository = _configurator.CreateRepository();
            var logger = _configurator.CreateLogger();
            var stopwatch = new Stopwatch();
            var itemStopwatch = new Stopwatch();

            stopwatch.Start();

            for (var i = 0; i < count; i++)
            {
                itemStopwatch.Reset();
                itemStopwatch.Start();
                repository.Add(CreateItem(_configurator.BlockSize));

                logger.Debug(string.Format("Add element {0}: took {1}", i, itemStopwatch.Elapsed));
            }

            stopwatch.Stop();

            logger.Info(string.Format("Add of {0} elements took {1}.", count, stopwatch.Elapsed));
            logger.Flush();
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(10000)]
        [TestCase(20000)]
        [TestCase(30000)]
        [TestCase(40000)]
        [TestCase(50000)]
        [TestCase(60000)]
        [TestCase(100000)]
        [Description("Just add so many items and calculate enumeration speed for all items ids.")]
        [Explicit]
        public void PerformanceTest_Ids(int count)
        {
            var repository = _configurator.CreateRepository();
            var logger = _configurator.CreateLogger();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            for (var i = 0; i < count; i++)
            {
                repository.Add(CreateItem(_configurator.BlockSize));
            }

            logger.Info(string.Format("Generating repo took: {0}", stopwatch.Elapsed));
            stopwatch.Reset();
            stopwatch.Start();

            repository.Ids.ToList();

            stopwatch.Stop();

            logger.Info(string.Format("Enumeration of all ({0}) elements took {1}.", count, stopwatch.Elapsed));
            logger.Flush();
        }

        [TestCase(100000)]
        [TestCase(60000)]
        [TestCase(50000)]
        [TestCase(40000)]
        [TestCase(30000)]
        [TestCase(20000)]
        [TestCase(10000)]
        [TestCase(1000)]
        [TestCase(100)]
        [TestCase(10)]
        [TestCase(1)]
        [Description("Just add so many items and delete them all and calculate deletion speed for all items.")]
        [Explicit]
        public void PerformanceTest_Delete(int count)
        {
            var repository = _configurator.CreateRepository();
            var ids = new List<ulong>();
            var logger = _configurator.CreateLogger();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (var i = 0; i < count; i++)
            {
                ids.Add(repository.Add(CreateItem(_configurator.BlockSize)));
            }

            logger.Info(string.Format("Generating repo took: {0}", stopwatch.Elapsed));

            stopwatch.Reset();
            stopwatch.Start();

            foreach (var id in ids)
            {
                repository.Delete(id);
            }

            stopwatch.Stop();

            logger.Info(string.Format("Deletion of all ({0}) elements took {1}.", count, stopwatch.Elapsed));
            logger.Flush();
        }

        [TestCase(100000)]
        [TestCase(60000)]
        [TestCase(50000)]
        [TestCase(40000)]
        [TestCase(30000)]
        [TestCase(20000)]
        [TestCase(10000)]
        [TestCase(1000)]
        [TestCase(100)]
        [TestCase(10)]
        [TestCase(1)]
        [Description("Just add so many items, close and reopen the repository and calculate reopen speed (mainly cache generation).")]
        [Explicit]
        public void PerformanceTest_ReopenRepository(int count)
        {
            var repository = _configurator.CreateRepository();
            var ids = new List<ulong>();
            var logger = _configurator.CreateLogger();
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (var i = 0; i < count; i++)
            {
                ids.Add(repository.Add(CreateItem(_configurator.BlockSize)));
            }

            logger.Info(string.Format("Generating repo took: {0}", stopwatch.Elapsed));

            stopwatch.Reset();
            stopwatch.Start();

            repository = _configurator.ReopenRepository(repository);

            repository.GetById(1);

            stopwatch.Stop();

            logger.Info(string.Format("Reopen of the repository with ({0}) elements took {1}.", count, stopwatch.Elapsed));
            logger.Flush();
        }

        #endregion

        #region Help methods

        private byte[] CreateItem(int size)
        {
            var buffer = new byte[size];

            _random.NextBytes(buffer);

            return buffer;
        }

        #endregion

        private class Configurator
        {
            private readonly IList<Tuple<string, Stream, StreamRepository<byte[]>>> _storageFiles;

            public Configurator()
            {
                _storageFiles = new List<Tuple<string, Stream, StreamRepository<byte[]>>>();
            }

            public enum StorageType
            {
                MemoryStream,
                File
            }

            public int Seed { get; set; }

            public StorageType Storage { get; set; }

            public LogLevel LogLevel { get; set; }

            public FlushPolicy LogFlushPolicy { get; set; }

            public int BlockSize { get; set; }

            public override string ToString()
            {
                var msg = new StringBuilder();

                msg
                    .AppendLine("=== Test Fixture Configuration ===")
                    .AppendLine("Seed: " + Seed)
                    .AppendLine("Storage type: " + Storage)
                    .AppendLine("Logger Level: " + LogLevel)
                    .AppendLine("Logger Flush policy: " + LogFlushPolicy)
                    .AppendLine("Block size: " + BlockSize);

                return msg.ToString();
            }

            public StreamRepository<byte[]> CreateRepository()
            {
                Stream stream;
                var path = string.Empty;

                if (Storage == StorageType.MemoryStream)
                {
                    stream = new MemoryStream();
                }
                else
                {
                    path = Path.GetTempFileName();
                    stream = File.Open(path, FileMode.Open);
                }

                var repo = new StreamRepository<byte[]>(stream,
                                                    new DataStreamMapper(BlockSize),
                                                    BlockSize);

                _storageFiles.Add(new Tuple<string, Stream, StreamRepository<byte[]>>(path, stream, repo));

                return repo;
            }

            public StreamRepository<byte[]> ReopenRepository(StreamRepository<byte[]> repository)
            {
                var item = _storageFiles.Single(x => x.Item3 == repository);
                
                _storageFiles.Remove(item);

                var repo = new StreamRepository<byte[]>(item.Item2,
                                                    new DataStreamMapper(BlockSize),
                                                    BlockSize);

                _storageFiles.Add(new Tuple<string, Stream, StreamRepository<byte[]>>(item.Item1, item.Item2, repo));

                return repo;
            }

            public Logger CreateLogger()
            {
                return new Logger(LogLevel, LogFlushPolicy);
            }

            public void Clear()
            {
                foreach (var storageFile in _storageFiles)
                {
                    storageFile.Item2.Close();

                    if (!string.IsNullOrWhiteSpace(storageFile.Item1))
                    {
                        File.Delete(storageFile.Item1);
                    }
                }
            }
        }
    }
}
