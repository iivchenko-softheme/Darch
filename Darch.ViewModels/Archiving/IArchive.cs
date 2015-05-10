// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Collections.Generic;
using Darch.Deduplication.Maps;

namespace Darch.ViewModels.Archiving
{
    public interface IArchive
    {
        IEnumerable<ArchiveFile> Files { get; }

        IMapProcessor Add(string path);
        
        IMapProcessor Remove(string name);

        IMapProcessor Extract(string name);

        void Flush();

        void Close();
    }
}
