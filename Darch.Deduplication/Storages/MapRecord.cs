// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Darch.Deduplication.Storages
{
    /// <summary>
    /// Represents item in the map storage.
    /// </summary>
    public sealed class MapRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapRecord"/> class.
        /// </summary>
        /// <param name="blockId">The identifier of the block in the map.</param>
        public MapRecord(ulong blockId)
        {
            BlockId = blockId;
        }

        /// <summary>
        /// Gets the identifier of the block in the map.
        /// </summary>
        public ulong BlockId { get; private set; }
    }
}
