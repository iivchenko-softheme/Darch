// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Diagnostics;
using Darch.Deduplication.Maps;

namespace Darch.Deduplication.Tests
{
    public sealed class MapMonitor : IDisposable
    {
        private readonly IMapProcessor _map;
        private readonly ulong _step;
        
        private readonly string _name;
        private readonly Stopwatch _stopwatch;

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
            _stopwatch = new Stopwatch();

            _previousValue = 0;

            _map.ProgressChanged += OnProgress;
            _map.StatusChanged += OnStatus;
        }

        public void OnProgress(object sender, ProgressEventArgs args)
        {
            var newValue = (args.Progress.WorkDone * 100) / args.Progress.WorkTotal;

            if (newValue >= _previousValue + _step || newValue == 100)
            {
                Console.WriteLine("[{0}] [{1}] Done: {2}% Time: {3}", _map.Id, _name, newValue, _stopwatch.Elapsed);
                
                _previousValue = newValue;
            }
        }

        public void OnStatus(object sender, StatusEventArgs args)
        {
            switch (args.Status)
            {
                case MapStatus.InProgress:
                    _stopwatch.Start();
                    Console.WriteLine("[{0}] [{1}] status changed to '{2}'", _map.Id, _name, args.Status);
                    break;
                
                case MapStatus.Canceled:
                case MapStatus.Failed:
                case MapStatus.Succeeded:
                    _stopwatch.Stop();
                    Console.WriteLine("[{0}] [{1}] status changed to '{2}'. Process took: {3}", _map.Id, _name, args.Status, _stopwatch.Elapsed);
                    break;

                case MapStatus.Paused:
                    _stopwatch.Stop();
                    break;
            }
        }

        public void Dispose()
        {
            _map.ProgressChanged -= OnProgress;
            _map.StatusChanged -= OnStatus;
        }
    }
}
