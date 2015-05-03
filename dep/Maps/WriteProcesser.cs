// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.IO;
using Deduplication.Storages;

namespace Deduplication.Maps
{
    public sealed class WriteProcessor : BaseProcessor
    {
        private readonly int _blockSize;
        private readonly Stream _source;

        public WriteProcessor(ulong mapId, int blockSize, Stream source, IStorage storage)
            : base(mapId, storage)
        {
            _source = source;
            _blockSize = blockSize;
        }

        protected override void InternalAction()
        {
            WaitResume();

            if (Canceled)
            {
                OnStatusChanged(new StatusEventArgs(MapStatus.Canceled));
                return;
            }

            var totalWork = (ulong)_source.Length;
            ulong doneWork = 0u;

            ProgressInternal = new Progress(totalWork, doneWork);

            while (true)
            {
                WaitResume();

                if (Canceled)
                {
                    OnStatusChanged(new StatusEventArgs(MapStatus.Canceled));
                    return;
                }

                var buffer = new byte[_blockSize];
                var bytesRead = _source.Read(buffer, 0, _blockSize);

                if (bytesRead == 0)
                {
                    OnStatusChanged(new StatusEventArgs(MapStatus.Succeeded));
                    return;
                }

                Storage.AddBlockItem(Id, buffer, bytesRead);

                doneWork = doneWork + (ulong)bytesRead;
                ProgressInternal = new Progress(totalWork, doneWork);
            }
        }
    }
}
