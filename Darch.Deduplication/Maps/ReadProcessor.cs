// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.IO;
using System.Linq;
using Darch.Deduplication.Storages;

namespace Darch.Deduplication.Maps
{
    /// <summary>
    /// Read data from the specified map.
    /// </summary>
    public sealed class ReadProcessor : BaseProcessor
    {
        private readonly Stream _target;

        public ReadProcessor(ulong mapId, Stream target, IStorage storage)
            : base(mapId, storage)
        {
            _target = target;
        }

        protected override void InternalAction()
        {
            WaitResume();

            if (Status == MapStatus.Canceling)
            {
                Status = MapStatus.Canceled;
                return;
            }

            // TODO: Do all calculations in bytes
            var mapItems = Storage.ReadMap(Id).ToList();
            var totalWork = (ulong)mapItems.Count;
            var doneWork = 0u;

            Progress = new Progress(totalWork, doneWork);

            foreach (var blockIndex in mapItems)
            {
                WaitResume();

                if (Status == MapStatus.Canceling)
                {
                    Status = MapStatus.Canceled;
                    return;
                }

                var block = Storage.ReadBlockItem(Id, blockIndex);

                _target.Write(block, 0, block.Length);

                Progress = new Progress(totalWork, ++doneWork);
            }

            Status = MapStatus.Succeeded;
        }
    }
}
