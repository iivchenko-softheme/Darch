// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using Deduplication.Maps;

namespace Deduplication.Tests
{
    public sealed class MapMonitor : IDisposable
    {
        private readonly IMapProcessor _map;
        private readonly ulong _step;
        private readonly string _name;

        private ulong _previousValue;

        public MapMonitor(IMapProcessor map)
            : this(map, 20)
        {
        }

        public MapMonitor(IMapProcessor map, ulong step)
        {
            _map = map;
            _step = step;

            _name = _map.GetType().Name;
            _previousValue = 0;

            _map.ProgressChanged += OnProgress;
            _map.StatusChanged += OnStatus;
        }

        public void OnProgress(object sender, ProgressEventArgs args)
        {
            var newValue = (args.Progress.WorkDone * 100) / args.Progress.WorkTotal;

            if (newValue >= _previousValue + _step || newValue == 100)
            {
                Console.WriteLine("[{0}] [{1}] Total: {2}%; Done: {3}", _map.Id, _name, 100, newValue);

                _previousValue = newValue;
            }
        }

        public void OnStatus(object sender, StatusEventArgs args)
        {
            Console.WriteLine("[{0}] [{1}] status changed to '{2}'", _map.Id, _name, args.Status);
        }

        public void Dispose()
        {
            _map.ProgressChanged -= OnProgress;
            _map.StatusChanged -= OnStatus;
        }
    }
}
