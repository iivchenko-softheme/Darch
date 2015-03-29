// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Deduplication
{
    /// <summary>
    /// Provides functionality to create storage.
    /// </summary>
    public interface IStorageFactory
    {
        RepositoryLib.IRepository<byte[], ulong> Create();
    }
}
