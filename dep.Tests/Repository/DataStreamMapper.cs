// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using RepositoryLib;

namespace Deduplication.Tests.Repository
{
    public class DataStreamMapper : IStreamMapper<byte[]>
    {
        private readonly int _bufferSize;

        public DataStreamMapper(int bufferSize)
        {
            _bufferSize = bufferSize;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#", Justification = "Special case when two methods become one.")]
        public byte[] Convert(byte[] buffer)
        {
            ValidateSize(buffer.Length);

            return buffer;
        }

        private void ValidateSize(int size)
        {
            if (size != _bufferSize)
            {
                throw new InvalidOperationException(string.Format("Specified buffer size '{0}' doesn't correspond to an expected size '{1}'.", size, _bufferSize));
            }
        }
    }
}
