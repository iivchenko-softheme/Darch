// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Collections.Generic;

namespace RepositoryLib
{
    /// <summary>
    /// Represents generic mechanism to manipulate with abstract storage.
    /// </summary>
    /// <typeparam name="TItem">Type that should be stored in storage.</typeparam>
    /// <typeparam name="TId">Type that represents unique identifier for each item in storage.</typeparam>
    public interface IRepository<TItem, TId>
    {
        /// <summary>
        /// Gets number of items in the storage.
        /// </summary>
        uint Count { get; }
        
        /// <summary>
        /// Gets enumerator to enumerate each item of the storage.
        /// </summary>
        IEnumerable<TItem> All { get; }

        /// <summary>
        /// Returns an item by its identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the item.</param>
        /// <returns>Return item from the storage.</returns>
        TItem GetById(TId id);

        /// <summary>
        /// Adds new item to the storage.
        /// </summary>
        /// <param name="item">New item to store in the repository.</param>
        /// <returns>Unique identifier for the new item.</returns>
        TId Add(TItem item);

        /// <summary>
        /// Deletes item from the storage by its identifier.
        /// </summary>
        /// <param name="id">Unique identifier of the item.</param>
        void Delete(TId id);

        /// <summary>
        /// Find item by identifier and replace it with new item.
        /// </summary>
        /// <param name="id">Unique identifier of the item.</param>
        /// <param name="item">New item that must replace the old one.</param>
        void Update(TId id, TItem item);
    }
}
