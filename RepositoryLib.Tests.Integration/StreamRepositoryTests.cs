// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace RepositoryLib.Tests.Integration
{
    [TestFixture]
    public class StreamRepositoryTests
    {
        private StreamRepository<StreamRepositoryTestItem> _repository;
        private Random _random;

        [TestFixtureSetUp]
        public void Initialize()
        {
            var seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Seed: {0}", seed);

            _random = new Random(seed);
        }

        [SetUp]
        public void Setup()
        {
            _repository = new StreamRepository<StreamRepositoryTestItem>(
                new MemoryStream(), 
                new StreamRepositoryTestMapper(),
                StreamRepositoryTestItem.ItemSize);
        }

        #region Functionality tests

        [Test]
        public void Test()
        {
            var mapper = new StreamRepositoryTestMapper();
            var item = new StreamRepositoryTestItem
            {
                TestValue1 = int.MinValue,
                TestValue2 = long.MaxValue,
                TestString = "Helloworld"
            };

            var buffer = mapper.Convert(item);
            var itemNew = mapper.Convert(buffer);

            Console.WriteLine(item.ToString());
            Console.Write("   ");
            Console.WriteLine(itemNew.ToString());
        }

        [Test]
        [Description("Add specific amount of items. It must be equals to the Count in the repository.")]
        public void AddCount_Test()
        {
            const int ItemsCount = 1000;
            for (var i = 0; i < ItemsCount; i++)
            {
                _repository.Add(CreateItem());
            }

            Assert.AreEqual(ItemsCount, _repository.Count);
        }

        [Test]
        [Description("Add soume items to the repository and then delete some. Repository count should be correct.")]
        public void AddDeleteCount_Test()
        {
            const int ItemsCountAdd = 10000;
            const int ItemsCountDelete = 251;

            var ids = new List<uint>();

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                ids.Add(_repository.Add(CreateItem()));
            }

            // Delete some from the repository
            // And also delete that items from ids list to prevent deletion of the same item twice.
            for (var i = 0; i < ItemsCountDelete; i++)
            {
                var id  = ids[_random.Next(ids.Count - 1)];
                ids.Remove(id);

                _repository.Delete(id);
            }

            Assert.AreEqual(ItemsCountAdd - ItemsCountDelete, _repository.Count);
        }

        // count test
        // add get by id test
        // add delete test
        // delete not existing item
        // add get by id and All test
        // add get by id update and get by id
        #endregion

        #region Performance tests
        #endregion

        #region Help methods

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
