// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace RepositoryLib
{
    public interface IStreamMapper<TItem>
    {
        byte[] Convert(TItem item);

        TItem Convert(byte[] buffer);
    }
}
