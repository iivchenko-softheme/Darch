// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Deduplication.Storages
{
    /// <summary>
    /// Contains metadata of a data block.
    /// </summary>
    public sealed class MetadataItem
    {
        /// <summary>
        /// Gets or sets the identifier of the  item.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Gets or sets identifier of the data block in the data repository.
        /// </summary>
        public ulong DataId { get; set; }

        /// <summary>
        /// Gets or sets data block checksum.
        /// </summary>
        public byte[] Checksum { get; set; }

        /// <summary>
        /// Gets or sets the real size (length) of the data block.
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// Gets or sets number of references (real usages) of the data block.
        /// </summary>
        public ulong References { get; set; }
    }
}
