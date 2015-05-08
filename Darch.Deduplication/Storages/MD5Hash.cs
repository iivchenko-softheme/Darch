// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Security.Cryptography;

namespace Darch.Deduplication.Storages
{
    public class MD5Hash : IHash
    {
        public byte[] Calculate(byte[] data)
        {
            using (var md5 = new MD5Cng())
            {
                return md5.ComputeHash(data);
            }
        }
    }
}
