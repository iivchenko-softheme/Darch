// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.IO;

namespace Deduplication.Maps
{
    public interface IMapProcessorFactory
    {
        IMapProcessor CreateReadProcessor(ulong mapId, Stream target);

        IMapProcessor CreateWriteProcessor(ulong mapId, ulong blockSize, Stream source);

        IMapProcessor CreateDeleteProcessor(ulong mapId);
    }
}
