// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Collections.Generic;
using System.IO;

namespace Deduplication
{
    /// <summary>
    /// Provides functionality to manipulate the entire repository.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Gets ids for all registered maps in the repository.
        /// </summary>
        IEnumerable<ulong> Maps { get; }

        /// <summary>
        /// Opens and prepares the repository for work.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the repository and releases its resources.
        /// </summary>
        void Close();

        /// <summary>
        /// Create new map, registers it the repository and create map writer monitor.
        /// </summary>
        /// <param name="input">Data stream which will be used as a source of data to write to the repository. All data from the stream will be written to the repository.</param>
        /// <returns>Data writing monitor.</returns>
        IMapProcessor Write(Stream input);

        /// <summary>
        /// Find map by id and creates map reader monitor.
        /// </summary>
        /// <param name="mapId">Identifier of the registered map in the repository.</param>
        /// <param name="output">Target data stream which where data from the map will writer. Stream will be reinitialized: cleared and writing will began from the very beginning.</param>
        /// <returns>Data reading monitor.</returns>
        IMapProcessor Read(ulong mapId, Stream output);

        /// <summary>
        /// Find map by id and creates map deleting monitor. After deleting the map will be deregistered.
        /// </summary>
        /// <param name="mapId">Identifier of the registered map in the repository.</param>
        /// <returns>Data deleting monitor.</returns>
        IMapProcessor Delete(ulong mapId);
    }
}
