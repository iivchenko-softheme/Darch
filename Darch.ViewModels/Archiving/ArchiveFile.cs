// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;

namespace Darch.ViewModels.Archiving
{
    [Serializable]
    public sealed class ArchiveFile
    {
        public string Name { get; set; }

        public ulong Id { get; set; }

        public long Length { get; set; }
    }
}
