// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shogun.Patterns.Repositories;
using Shogun.Patterns.Tests.Integration.Repositories;
using Shogun.Patterns.Tests.Performance.Utility.Logging;

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
                Storage = Configurator.StorageType.MemoryStream,
                LogLevel = LogLevel.Trace,
                LogFlushPolicy = FlushPolicy.Buffered
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
            var repository = CreateRepository();
            var logger = _configurator.CreateLogger();
            var stopwatch = new Stopwatch();
            var itemStopwatch = new Stopwatch();

            stopwatch.Start();

            for (var i = 0; i < count; i++)
            {
                itemStopwatch.Reset();
                itemStopwatch.Start();
                repository.Add(CreateItem());

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
        [Description("Just add so many items and calculate enumeration speed for all items.")]
        [Explicit]
        public void PerformanceTest_All(int count)
        {
            var repository = CreateRepository();
            var stopwatch = new Stopwatch();

            for (var i = 0; i < count; i++)
            {
                repository.Add(CreateItem());
            }

            stopwatch.Start();

            foreach (var item in repository.All)
            {
                item.Id.ToString(CultureInfo.InvariantCulture);
            }

            stopwatch.Stop();

            Console.WriteLine("Enumeration of all ({0}) elements took {1}.", count, stopwatch.Elapsed);
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
            var repository = CreateRepository();
            var ids = new List<ulong>();
            var stopwatch = new Stopwatch();

            for (var i = 0; i < count; i++)
            {
                ids.Add(repository.Add(CreateItem()));
            }

            stopwatch.Start();

            foreach (var id in ids)
            {
                repository.Delete(id);
            }

            stopwatch.Stop();

            Console.WriteLine("Deletion of all ({0}) elements took {1}.", count, stopwatch.Elapsed);
        }

        #endregion

        #region Help methods
        
        private static StreamRepository<StreamRepositoryTestItem> CreateRepository(Stream stream)
        {
            return new StreamRepository<StreamRepositoryTestItem>(
                stream,
                new StreamRepositoryTestMapper(),
                StreamRepositoryTestItem.ItemSize);
        }

        private StreamRepository<StreamRepositoryTestItem> CreateRepository()
        {
            return CreateRepository(_configurator.CreateStorage());
        }

        private StreamRepositoryTestItem CreateItem()
        {
            var buffer = new byte[StreamRepositoryTestItem.TestStringSize];

            _random.NextBytes(buffer);

            var str = new string(buffer.Select(x => (char)x).ToArray());

            return new StreamRepositoryTestItem
            {
                TestString = str,
                TestValue1 = _random.Next(),
                TestValue2 = _random.Next()
            };
        }

        #endregion

        private class Configurator
        {
            public enum StorageType
            {
                MemoryStream,
                File
            }

            public int Seed { get; set; }

            public StorageType Storage { get; set; }
            
            public LogLevel LogLevel { get; set; }

            public FlushPolicy LogFlushPolicy { get; set; }

            public override string ToString()
            {
                var msg = new StringBuilder();

                msg
                    .AppendLine("=== Test Fixture Configuration ===")
                    .AppendLine("Seed: " + Seed)
                    .AppendLine("Storage type: " + Storage)
                    .AppendLine("Logger Level: " + LogLevel)
                    .AppendLine("Logger Flush policy: " + LogFlushPolicy);

                return msg.ToString();
            }

            public Stream CreateStorage()
            {
                return new MemoryStream();
            }

            public Logger CreateLogger()
            {
                return new Logger(LogLevel, LogFlushPolicy);
            }

            public void Clear()
            {
            }
        }
    }
}
