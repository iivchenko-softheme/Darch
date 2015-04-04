// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Deduplication.Storages
{
    /// <summary>
    /// Represents item in the map storage.
    /// </summary>
    public class MapRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapRecord"/> class.
        /// </summary>
        /// <param name="mapId">The identifier of the map.</param>
        /// <param name="blockId">The identifier of the block in the map.</param>
        /// <param name="recordIndex">The index number of the map record in the map.</param>
        public MapRecord(ulong mapId, ulong blockId, ulong recordIndex)
        {
            MapId = mapId;
            BlockId = blockId;
            RecordIndex = recordIndex;
        }

        /// <summary>
        /// Gets the identifier of the map.
        /// </summary>
        public ulong MapId { get; private set; }

        /// <summary>
        /// Gets the identifier of the block in the map.
        /// </summary>
        public ulong BlockId { get; private set; }

        /// <summary>
        /// Gets the index number of the map record in the map.
        /// </summary>
        public ulong RecordIndex { get; private set; }
    }
}
