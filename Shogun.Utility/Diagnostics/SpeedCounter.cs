// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Shogun.Utility.Diagnostics
{
    /// <summary>
    /// Provides functionality to measure speed of execution for methods and delegates.
    /// Provides grouping to separate statistics for different methods and delegates.
    /// </summary>
    public sealed class SpeedCounter
    {
        private readonly Stopwatch _stopwatch;
        private readonly IDictionary<string, SpeedStatistics> _statistics;
        
        public SpeedCounter()
        {
            _stopwatch = new Stopwatch();
            _statistics = new Dictionary<string, SpeedStatistics>();
        }

        /// <summary>
        /// Gets the speed statistics divided by groups.
        /// </summary>
        public IReadOnlyDictionary<string, SpeedStatistics> Statistics
        {
            get
            {
                return new ReadOnlyDictionary<string, SpeedStatistics>(_statistics);
            }
        }

        /// <summary>
        /// Measure speed of the method/delegate and place results to the specified group.
        /// If the group doesn't exist so it will be created.
        /// </summary>
        public void Measure(string name, Action method)
        {
            MeasureInternal<object>(
                name,
                () =>
                {
                    method();

                    return null;
                });
        }

        /// <summary>
        /// Measure speed of the method/delegate and place results to the specified group.
        /// If the group doesn't exist so it will be created.
        /// </summary>
        /// <typeparam name="T">Type of the method/delegate result.</typeparam>
        public T Measure<T>(string name, Func<T> method)
        {
            return MeasureInternal(name, method);
        }

        private T MeasureInternal<T>(string name, Func<T> method)
        {
            _stopwatch.Reset();
            _stopwatch.Start();

            var result = method();

            _stopwatch.Stop();

            if (!_statistics.ContainsKey(name))
            {
                _statistics.Add(name, new SpeedStatistics(name));
            }

            _statistics[name].Add(_stopwatch.Elapsed);

            return result;
        }
    }
}
