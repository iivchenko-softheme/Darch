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
using Shogun.Patterns.Repositories;

namespace Shogun.Patterns.Tests.Integration.Repositories
{
    [TestFixture]
    public class StreamRepositoryTests
    {
        private const int ItemsCountAdd = 1000;
        private const int ItemsCountDelete = 251;
        
        private Random _random;

        [TestFixtureSetUp]
        public void Initialize()
        {
            var seed = (int)DateTime.Now.Ticks;

            Console.WriteLine("Seed: {0}", seed);

            _random = new Random(seed);
        }

        #region Functionality tests
      
        [Test]
        [Description("Add specific amount of items. It must be equals to the Count in the repository.")]
        public void AddCount_Test()
        {
            var repository = CreateRepository();

            for (var i = 0; i < ItemsCountAdd; i++)
            {
                repository.Add(CreateItem());
            }

            Assert.AreEqual(ItemsCountAdd, repository.Count);
        }

        [Test]
        [Description("Add soume items to the repository and then delete some. Repository count should be correct.")]
        public void AddDeleteCount_Test()
        {
            var repository = CreateRepository();

            var ids = new List<ulong>();

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                ids.Add(repository.Add(CreateItem()));
            }

            // Delete some items from the repository
            // And also delete that items from ids list to prevent deletion of the same item twice.
            for (var i = 0; i < ItemsCountDelete; i++)
            {
                var id  = ids[_random.Next(ids.Count - 1)];
                ids.Remove(id);

                repository.Delete(id);
            }

            Assert.AreEqual(ItemsCountAdd - ItemsCountDelete, repository.Count);
        }

        [Test]
        [Description("Add specific amount of items. Enumerate All repository items and check that they are equals.")]
        public void AddIds_Test()
        {
            var repository = CreateRepository();
            var items = new List<RepositoryItem<ulong, StreamRepositoryTestItem>>();

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                var item = CreateItem();
                
                var id = repository.Add(item);
                items.Add(new RepositoryItem<ulong, StreamRepositoryTestItem>(id, item));
            }

            // Check if items are same in the repo and in the list
            foreach (var id in repository.Ids)
            {
                var expected = items.Single(x => x.Id == id).Item;
                var actual = repository.GetById(id);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        [Description("Add specific amount of items. Delete some of them. Enumerate All repository items and check that they are equals.")]
        public void AddDeleteAll_Test()
        {
            var repository = CreateRepository();
            var items = new List<RepositoryItem<ulong, StreamRepositoryTestItem>>();

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                var item = CreateItem();

                var id = repository.Add(item);
                items.Add(new RepositoryItem<ulong, StreamRepositoryTestItem>(id, item));
            }

            // Delete some items from the repository
            // And also delete that items from ids list to prevent deletion of the same item twice.
            for (var i = 0; i < ItemsCountDelete; i++)
            {
                var item = items[_random.Next(items.Count - 1)];
                items.Remove(item);

                repository.Delete(item.Id);
            }

            // Check if items are same in the repo and in the list
            foreach (var id in repository.Ids)
            {
                var expected = items.Single(x => x.Id == id).Item;
                var actual = repository.GetById(id);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        [Description("Add specific amount of items and get them by id and check that they are equals.")]
        public void AddGetById_Test()
        {
            var repository = CreateRepository();
            var items = new List<RepositoryItem<ulong, StreamRepositoryTestItem>>();

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                var item = CreateItem();

                var id = repository.Add(item);
                items.Add(new RepositoryItem<ulong, StreamRepositoryTestItem>(id, item));
            }

            // Check if items are same in the repo and in the list
            foreach (var expected in items)
            {
                var actual = repository.GetById(expected.Id);

                Assert.AreEqual(expected.Item, actual);
            }
        }

        [Test]
        [Description("Add specific amount of items. Delete some of them. Get items by id and check that they are equals.")]
        public void AddDeleteGetById_Test()
        {
            var repository = CreateRepository();
            var items = new List<RepositoryItem<ulong, StreamRepositoryTestItem>>();

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                var item = CreateItem();

                var id = repository.Add(item);
                items.Add(new RepositoryItem<ulong, StreamRepositoryTestItem>(id, item));
            }

            // Delete some items from the repository
            // And also delete that items from ids list to prevent deletion of the same item twice.
            for (var i = 0; i < ItemsCountDelete; i++)
            {
                var item = items[_random.Next(items.Count - 1)];
                items.Remove(item);

                repository.Delete(item.Id);
            }

            // Check if items are same in the repo and in the list
            foreach (var expected in items)
            {
                var actual = repository.GetById(expected.Id);

                Assert.AreEqual(expected.Item, actual);
            }
        }

        [Test]
        [Description("Add some items. Check them added. Update some of that items. Check they updated correctly")]
        public void AddUpdate_Test()
        {
            var repository = CreateRepository();
            var items = new List<RepositoryItem<ulong, StreamRepositoryTestItem>>();
            var updatedItems = new List<RepositoryItem<ulong, StreamRepositoryTestItem>>(); 

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                var item = CreateItem();

                var id = repository.Add(item);
                items.Add(new RepositoryItem<ulong, StreamRepositoryTestItem>(id, item));
            }

            // Check if items are same in the repo and in the list
            foreach (var expected in items)
            {
                var actual = repository.GetById(expected.Id);

                Assert.AreEqual(expected.Item, actual);
            }

            // Update all items in the repository
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                var item = CreateItem();
                var id = items[i].Id;
                updatedItems.Add(new RepositoryItem<ulong, StreamRepositoryTestItem>(id, item));
                
                repository.Update(id, item);
            }

            // Check if items updated correctly
            foreach (var expected in updatedItems)
            {
                var actual = repository.GetById(expected.Id);

                Assert.AreEqual(expected.Item, actual);
            }

            Assert.AreEqual(ItemsCountAdd, repository.Count);
        }

        [Test]
        [Description("Add some items. Get by id not existing one. Should throw exception.")]
        public void GetById_NotExistingItem_Throws()
        {
            var repository = CreateRepository();
            var items = new List<RepositoryItem<ulong, StreamRepositoryTestItem>>();

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                var item = CreateItem();

                var id = repository.Add(item);
                items.Add(new RepositoryItem<ulong, StreamRepositoryTestItem>(id, item));
            }

            var notExistingId = items.Max(x => x.Id) + 1;

            Assert.Throws<MissingItemException>(() => repository.GetById(notExistingId));
        }

        [Test]
        [Description("Add some items. Delete not existing one. Should throw exception.")]
        public void Delete_NotExistingItem_Throws()
        {
            var repository = CreateRepository();
            var items = new List<RepositoryItem<ulong, StreamRepositoryTestItem>>();

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                var item = CreateItem();

                var id = repository.Add(item);
                items.Add(new RepositoryItem<ulong, StreamRepositoryTestItem>(id, item));
            }

            var idToDelete = items[_random.Next(items.Count - 1)].Id;
            repository.Delete(idToDelete);

            Assert.Throws<MissingItemException>(() => repository.Delete(idToDelete));
        }

        [Test]
        [Description("Delete the only item from the repository. Should work.")]
        public void Delete_TheOnlyItem_Test()
        {
            var repository = CreateRepository();
            var item = CreateItem();
            var id = repository.Add(item);

            repository.Delete(id);

            Assert.AreEqual(0, repository.Count);
        }

        [Test]
        [Description("Create and fill a repo. Create a new on based on the same stream to preload ids. Sure that all items not corrupted.")]
        public void PreloadAll_Test()
        {
            var stream = new MemoryStream();
            var repository = CreateRepository(stream);
            var items = new List<RepositoryItem<ulong, StreamRepositoryTestItem>>();

            // Fill the repository with items
            for (var i = 0; i < ItemsCountAdd; i++)
            {
                var item = CreateItem();

                var id = repository.Add(item);
                items.Add(new RepositoryItem<ulong, StreamRepositoryTestItem>(id, item));
            }

            repository = CreateRepository(stream); // To initiate preload algo

            // Check if items are same in the repo and in the list
            foreach (var expected in items)
            {
                var actual = repository.GetById(expected.Id);

                Assert.AreEqual(expected.Item, actual);
            }
        }

        // delete not existing item
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
