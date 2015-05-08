// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Globalization;

namespace Shogun.Patterns.Tests.Integration.Repositories
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

        public static bool operator ==(StreamRepositoryTestItem left, StreamRepositoryTestItem right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(StreamRepositoryTestItem left, StreamRepositoryTestItem right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}; {1}; {2}", TestValue1, TestValue2, _testString);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((StreamRepositoryTestItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _testString != null ? _testString.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ TestValue1;
                hashCode = (hashCode * 397) ^ TestValue2.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(StreamRepositoryTestItem obj)
        {
            return 
                string.Equals(_testString, obj._testString) && 
                TestValue1 == obj.TestValue1 &&
                TestValue2 == obj.TestValue2;
        }
    }
}
