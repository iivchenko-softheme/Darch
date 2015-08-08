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
using NUnit.Framework;
using Shogun.Patterns.Repositories;
using Shogun.Patterns.Tests.Integration.Repositories;

namespace Shogun.Patterns.Tests.Performance.Repositories
{
    [TestFixture]
    public class StreamRepositoryTests
    {
        private Random _random;

        [TestFixtureSetUp]
        public void Initialize()
        {
            var seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Seed: {0}", seed);

            _random = new Random(seed);
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
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            for (var i = 0; i < count; i++)
            {
                repository.Add(CreateItem());
            }

            stopwatch.Stop();

            Console.WriteLine("Add of {0} elements took {1}.", count, stopwatch.Elapsed);
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

        private static StreamRepository<StreamRepositoryTestItem> CreateRepository()
        {
            return CreateRepository(new MemoryStream());
        }

        private static StreamRepository<StreamRepositoryTestItem> CreateRepository(Stream stream)
        {
            return new StreamRepository<StreamRepositoryTestItem>(
                stream,
                new StreamRepositoryTestMapper(),
                StreamRepositoryTestItem.ItemSize);
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
    }
}
