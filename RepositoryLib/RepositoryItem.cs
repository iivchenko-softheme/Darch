// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace RepositoryLib
{
    /// <summary>
    /// Holder for <see cref="TItem"/> data and its <see cref="TId"/>.
    /// </summary>
    /// <typeparam name="TId">Type that represent <see cref="TItem"/> unique identifier.</typeparam>
    /// <typeparam name="TItem">Type that should be stored in the storage.</typeparam>
    public sealed class RepositoryItem<TId, TItem>
    {
        public RepositoryItem(TId id, TItem item)
        {
            Id = id;
            Item = item;
        }

        public TId Id { get; private set; }

        public TItem Item { get; private set; }
    }
}
