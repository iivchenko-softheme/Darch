// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using NUnit.Framework;

namespace Shogun.Utility.Tests
{
    [TestFixture]
    public class ValidateTests
    {
        [Test]
        public void Null_ParameterIsNotNull_DoNothing()
        {
            var parameter = new object();
            Validate.Null(parameter, "parameter");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null_ParameterIsNull_Throws()
        {
            var parameter = (object)null;
            Validate.Null(parameter, "parameter");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null_NameNotSpecified_Throws()
        {
            var parameter = new object();
            Validate.Null(parameter, null);
        }

        [Test]
        public void StringEmpty_StringIsNotEmpty_DoNothing()
        {
            var parameter = "Hello world";
            Validate.StringEmpty(parameter, "parameter");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringEmpty_StringIsNull_Throws()
        {
            string parameter = null;
            Validate.StringEmpty(parameter, "parameter");
        }
        
        [TestCase("")]
        [TestCase("   ")]
        [ExpectedException(typeof(ArgumentException))]
        public void StringEmpty_StringIsEmpty_Throws(string input)
        {
            Validate.StringEmpty(input, "input");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StringEmpty_NameNotSpecified_Throws()
        {
            var parameter = "Hello world";
            Validate.StringEmpty(parameter, null);
        }
    }
}
