// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.IO;
using Darch.Deduplication.Storages;
using Shogun.Utility.Jobs;

namespace Darch.Deduplication.Maps
{
    /// <summary>
    /// Writes data to the specified map.
    /// </summary>
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

            if (Status == JobStatus.Canceling)
            {
                Status = JobStatus.Canceled;
                return;
            }

            var totalWork = (ulong)_source.Length;
            ulong doneWork = 0u;

            Progress = new Progress(totalWork, doneWork);

            while (true)
            {
                WaitResume();

                if (Status == JobStatus.Canceling)
                {
                    Status = JobStatus.Canceled;
                    return;
                }

                var buffer = new byte[_blockSize];
                var bytesRead = _source.Read(buffer, 0, _blockSize);

                if (bytesRead == 0)
                {
                    Status = JobStatus.Succeeded;
                    return;
                }

                Storage.AddBlockItem(Id, buffer, bytesRead);

                doneWork = doneWork + (ulong)bytesRead;
                Progress = new Progress(totalWork, doneWork);
            }
        }
    }
}
