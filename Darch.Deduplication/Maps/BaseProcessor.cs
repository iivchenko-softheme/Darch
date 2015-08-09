// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Threading;
using System.Threading.Tasks;
using Darch.Deduplication.Storages;
using Darch.Deduplication.Utility;
using Shogun.Utility.Extensions;
using Shogun.Utility.Jobs;

namespace Darch.Deduplication.Maps
{
    /// <summary>
    /// Infrastructure for different map processors. Provides Status, Progress, Start, Pause, Resume, Cancel etc. API.
    /// </summary>
    public abstract class BaseProcessor : Job
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseProcessor"/> class.
        /// </summary>
        /// <param name="mapId">The identifier of the processing map.</param>
        /// <param name="storage">The storage with the data that is handled by the processor.</param>
        protected BaseProcessor(ulong mapId, IStorage storage)
        {
            Id = mapId;
            Storage = storage;
        }

        /// <summary>
        /// Gets access to the storage so derived processor can use it.
        /// </summary>
        protected IStorage Storage { get; private set; }
    }
}
