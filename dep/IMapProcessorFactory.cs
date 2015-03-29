// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Deduplication
{
    /// <summary>
    /// Provides functionality to create different handlers for data handling.
    /// </summary>
    public interface IMapProcessorFactory
    {
        /// <summary>
        /// Creates a processor to write data to a repository.
        /// </summary>
        IMapProcessor CreateWriteMap();

        /// <summary>
        /// Creates a processor to read data from a repository.
        /// </summary>
        /// <param name="id">The identifier of the map which data must be read.</param>
        IMapProcessor CreateReadMap(ulong id);

        /// <summary>
        /// Creates a processor to delete data from a repository.
        /// </summary>
        /// <param name="id">The identifier of the map which data must be deleted.</param>
        IMapProcessor CreateDeleteMap(ulong id);
    }
}
