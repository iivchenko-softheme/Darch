// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Threading;
using NUnit.Framework;
using Shogun.Utility.Diagnostics;

namespace Shogun.Utility.Tests.Diagnostics
{
    [TestFixture]
    public class SpeedCounterTests
    {
        [Test]
        public void Measure()
        {
            Action action = () => Thread.Sleep(5000);

            Func<int> func = () =>
            {
                Thread.Sleep(6000);

                return 666;
            };

            var counter = new SpeedCounter();

            counter.Measure("Action", action);
            var res = counter.Measure("Func", func);

            // Warning!!! Be aware of such hacks. I totaly know that there are no minutes and hours.
            Assert.AreEqual(5,  counter.Statistics["Action"].Total().Seconds);
            Assert.AreEqual(6, counter.Statistics["Func"].Total().Seconds);
            Assert.AreEqual(666, res);
        }
    }
}
