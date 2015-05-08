// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Darch.Deduplication.Tests.Data
{
    public sealed class FileInfo
    {
        public string Path { get; set; }

        public byte[] Hash { get; set; }

        public int Size { get; set; }

        public ulong MapId { get; set; }
    }
}
