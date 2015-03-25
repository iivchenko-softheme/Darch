// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RepositoryLib.Tests.Integration
{
    public class TestMapper : IStreamMapper<StreamRepositoryTestItem>
    {
        public byte[] Convert(StreamRepositoryTestItem item)
        {
            var buffer = new List<byte>();
            buffer.AddRange(BitConverter.GetBytes(item.TestValue1));
            buffer.AddRange(BitConverter.GetBytes(item.TestValue2));
            buffer.AddRange(item.TestString.Select(x => (byte)x));

            return buffer.ToArray();
        }

        public StreamRepositoryTestItem Convert(byte[] buffer)
        {
            if (buffer.Length != StreamRepositoryTestItem.ItemSize)
            {
                var msg = string.Format(CultureInfo.InvariantCulture, "Item size is '{0}' but MUST be '{1}'", buffer.Length, StreamRepositoryTestItem.ItemSize);
                
                throw new InvalidOperationException(msg);
            }

            return new StreamRepositoryTestItem
            {
                TestValue1 = BitConverter.ToInt32(buffer.Take(sizeof(int)).ToArray(), 0),
                TestValue2 = BitConverter.ToInt64(buffer.Skip(sizeof(int)).Take(sizeof(long)).ToArray(), 0),
                TestString = new string(buffer.Skip(sizeof(int) + sizeof(long)).Select(x => (char)x).ToArray())
            };
        }
    }
}
