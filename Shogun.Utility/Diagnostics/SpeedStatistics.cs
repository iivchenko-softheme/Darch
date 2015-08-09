// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Shogun.Utility.Diagnostics
{
    /// <summary>
    /// Provides speed (time measurement) execution statistics for method/delegate.
    /// </summary>
    public sealed class SpeedStatistics
    {
        private readonly IList<TimeSpan> _executionTimes;

        internal SpeedStatistics(string name)
        {
            Name = name;

            _executionTimes = new List<TimeSpan>();
        }

        /// <summary>
        /// Gets name of the statistics.
        /// </summary>
        public string Name { get; private set; }

        public IEnumerable<TimeSpan> ExecutionTimes
        {
            get
            {
                return _executionTimes;
            }
        }

        /// <summary>
        /// Add new measurement value to the statistics.
        /// </summary>
        public void Add(TimeSpan time)
        {
            _executionTimes.Add(time);
        }

        /// <summary>
        /// Returns number of added measurements.
        /// </summary>
        public int Count()
        {
            return _executionTimes.Count;
        }

        /// <summary>
        /// Returns the biggest measurement.
        /// </summary>
        public TimeSpan Max()
        {
            return _executionTimes.Max();
        }

        /// <summary>
        /// Returns the smallest measurement.
        /// </summary>
        public TimeSpan Min()
        {
            return _executionTimes.Min();
        }

        /// <summary>
        /// Counts and returns average measurement.
        /// </summary>
        public TimeSpan Average()
        {
            var sum =
                _executionTimes
                    .Select(x => x.Ticks)
                    .Sum();

            return new TimeSpan(sum / _executionTimes.Count);
        }

        /// <summary>
        /// Counts and returns sum of all measurements.
        /// </summary>
        public TimeSpan Total()
        {
            var sum =
                _executionTimes
                    .Select(x => x.Ticks)
                    .Sum();

            return new TimeSpan(sum);
        }
    }
}
