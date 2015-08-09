// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.IO;
using Shogun.Utility.Jobs;

namespace Darch.Deduplication.Maps
{
    public interface IMapProcessorFactory
    {
        IJob CreateReadProcessor(ulong mapId, Stream target);

        IJob CreateWriteProcessor(ulong mapId, int blockSize, Stream source);

        IJob CreateDeleteProcessor(ulong mapId);
    }
}
