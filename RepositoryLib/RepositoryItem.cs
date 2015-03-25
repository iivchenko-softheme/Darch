// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

// TODO: Override equals for tests
namespace RepositoryLib
{
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
