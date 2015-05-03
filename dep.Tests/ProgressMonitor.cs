// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using Deduplication.Maps;

namespace Deduplication.Tests
{
    public sealed class ProgressMonitor
    {
        private readonly string _name;
        private readonly ulong _step;

        private ulong _previousValue;

        public ProgressMonitor(string name, ulong step)
        {
            _name = name;
            _step = step;
            _previousValue = 0;
        }

        public void OnProgress(object sender, ProgressEventArgs args)
        {
            var newValue = (args.Progress.WorkDone * 100) / args.Progress.WorkTotal;

            if (newValue > _previousValue + _step || newValue == 100)
            {
                Console.WriteLine("[{0}] Total: {1}%; Done: {2}", _name, 100, newValue);

                _previousValue = newValue;
            }
        }

        public void OnStatus(object sender, StatusEventArgs args)
        {
            Console.WriteLine("New status: {0}", args.Status);
        }
    }
}
