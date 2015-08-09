// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Linq;
using Darch.Deduplication.Storages;
using Shogun.Utility.Jobs;

namespace Darch.Deduplication.Maps
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

            if (Status == JobStatus.Canceling)
            {
                Status = JobStatus.Canceled;
                return;
            }

            var mapItems = Storage.ReadMap(Id).ToList();
            var totalWork = (ulong)mapItems.Count;
            var doneWork = 0u;

            Progress = new Progress(totalWork, doneWork);

            foreach (var mapItemId in mapItems)
            {
                WaitResume();

                if (Status == JobStatus.Canceling)
                {
                    Status = JobStatus.Canceled;
                    return;
                }

                Storage.DeleteBlockItem(Id, mapItemId);

                Progress = new Progress(totalWork, ++doneWork);
            }

            Status = JobStatus.Succeeded;
        }
    }
}
