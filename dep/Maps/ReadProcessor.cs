// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.IO;
using System.Linq;
using Deduplication.Storages;

namespace Deduplication.Maps
{
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

            if (Canceled)
            {
                OnStatusChanged(new StatusEventArgs(MapStatus.Canceled));
                return;
            }

            // TODO: Do all calculations in bytes
            var mapItems = Storage.ReadMap(Id).ToList();
            var totalWork = (ulong)mapItems.Count;
            var doneWork = 0u;

            ProgressInternal = new Progress(totalWork, doneWork);

            foreach (var blockIndex in mapItems)
            {
                WaitResume();

                if (Canceled)
                {
                    OnStatusChanged(new StatusEventArgs(MapStatus.Canceled));
                    return;
                }

                var block = Storage.ReadBlockItem(Id, blockIndex);

                _target.Write(block, 0, block.Length);

                ProgressInternal = new Progress(totalWork, ++doneWork);
            }

            OnStatusChanged(new StatusEventArgs(MapStatus.Succeeded));
        }
    }
}
