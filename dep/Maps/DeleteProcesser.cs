// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Linq;
using Deduplication.Storages;

namespace Deduplication.Maps
{
    public sealed class DeleteProcessor : BaseProcessor
    {
        public DeleteProcessor(ulong mapId, IStorage storage)
            : base(mapId, storage)
        {
        }

        protected override void InternalAction()
        {
            WaitResume();

            if (Canceled)
            {
                OnStatusChanged(new StatusEventArgs(MapStatus.Canceled));
                return;
            }

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

                Storage.DeleteBlockItem(Id, blockIndex);

                ProgressInternal = new Progress(totalWork, ++doneWork);
            }

            OnStatusChanged(new StatusEventArgs(MapStatus.Succeeded));
        }
    }
}
