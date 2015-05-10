// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Collections.Generic;
using NUnit.Framework;
using Shogun.Utility.Extensions;

namespace Shogun.Utility.Tests.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void ForEach_Test()
        {
            var counter = 1;
            IEnumerable<int> source = new List<int> { 1, 2, 3, 4, 5 };

            source.ForEach(x => Assert.AreEqual(counter++, x));
        }
    }
}
