// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Linq;
using NUnit.Framework;
using Shogun.Utility.Diagnostics;

namespace Shogun.Utility.Tests.Diagnostics
{
    [TestFixture]
    public class SpeedStatisticsTests
    {
        [Test]
        public void General_Test()
        {
            var time1 = new TimeSpan(0, 0, 0, 59);
            var time2 = new TimeSpan(0, 0, 2, 1);
            var time3 = new TimeSpan(0, 1, 0, 0); // max
            
            var statistics = new SpeedStatistics("TestName");

            statistics.Add(time1);
            statistics.Add(time2);
            statistics.Add(time3);

            Assert.AreEqual("TestName", statistics.Name, "Name");

            Assert.AreEqual(time1, statistics.ExecutionTimes.ToList()[0], "time1");
            Assert.AreEqual(time2, statistics.ExecutionTimes.ToList()[1], "time2");
            Assert.AreEqual(time3, statistics.ExecutionTimes.ToList()[2], "time3");

            Assert.AreEqual(3, statistics.Count(), "Count();");
            
            Assert.AreEqual(time1, statistics.Min(), "Max();");
            Assert.AreEqual(time3, statistics.Max(), "Min();");
            Assert.AreEqual(new TimeSpan(0, 0, 21, 0), statistics.Average(), "Average();");
            Assert.AreEqual(new TimeSpan(0, 1, 2, 60), statistics.Total(), "Total();");
        }
    }
}
