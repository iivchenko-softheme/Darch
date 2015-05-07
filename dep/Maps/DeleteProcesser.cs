// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Linq;
using Deduplication.Storages;

namespace Deduplication.Maps
{
    /// <summary>
    /// Deletes specified map from the storage.
    /// </summary>
    public sealed class DeleteProcessor : BaseProcessor
    {
        public DeleteProcessor(ulong mapId, IStorage storage)
            : base(mapId, storage)
        {
        }

        protected override void InternalAction()
        {
            WaitResume();

            if (Status == MapStatus.Canceling)
            {
                Status = MapStatus.Canceled;
                return;
            }

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

                Storage.DeleteBlockItem(Id, blockIndex);

                Progress = new Progress(totalWork, ++doneWork);
            }

            Status = MapStatus.Succeeded;
        }
    }
}
