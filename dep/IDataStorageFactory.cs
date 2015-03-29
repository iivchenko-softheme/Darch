// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Deduplication
{
    /// <summary>
    /// Provides functionality to create data storage.
    /// </summary>
    public interface IDataStorageFactory
    {
        RepositoryLib.IRepository<byte[], ulong> Create();
    }
}
