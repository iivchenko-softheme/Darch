// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using NUnit.Framework;
using Shogun.Utility.Extensions;

namespace Shogun.Utility.Tests.Extensions
{
    [TestFixture]
    public class EventHandlerExtensionsTests
    {
        [Test]
        public void OnEvent_Test()
        {
            var eventRaised = false;
            var test = new TestClass();
            test.TestEvent += (sender, args) => eventRaised = true;

            test.OnTestEvent();

            Assert.True(eventRaised);
        }

        private class TestClass
        {
            public event EventHandler<EventArgs> TestEvent;

            public void OnTestEvent()
            {
                TestEvent.OnEvent(this, new EventArgs());
            }
        }
    }
}
