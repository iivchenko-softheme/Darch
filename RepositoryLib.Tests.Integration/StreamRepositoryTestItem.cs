// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Globalization;

namespace RepositoryLib.Tests.Integration
{
    public class StreamRepositoryTestItem
    {
        public const int ItemSize = sizeof(int) + sizeof(long) + TestStringSize;
        public const int TestStringSize = 10;

        private string _testString;

        public int TestValue1 { get; set; }

        public long TestValue2 { get; set; }

        public string TestString
        {
            get
            {
                return _testString;
            }

            set
            {
                if (value.Length != TestStringSize)
                {
                    var message =
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Test string should be '{0}' symbols length but it tried be '{1}' symbols length.",
                            TestStringSize, 
                            value.Length);

                    throw new InvalidOperationException(message);
                }

                _testString = value;
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}; {1}; {2}", TestValue1, TestValue2, TestString);
        }
    }
}
