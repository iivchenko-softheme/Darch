// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Collections.Generic;
using Darch.Deduplication.Maps;
using Shogun.Utility.Jobs;

namespace Darch.ViewModels.Archiving
{
    public interface IArchive
    {
        IEnumerable<ArchiveFile> Files { get; }

        IJob Add(string path);
        
        IJob Remove(string name);

        IJob Extract(string name);

        void Flush();

        void Close();
    }
}
