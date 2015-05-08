// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Shogun.Patterns.Repositories
{
    /// <summary>
    /// Provides conversation of <see cref="TItem"/> to a byte array and a byte array to an <see cref="TItem"/>
    /// </summary>
    /// <typeparam name="TItem">Type that should be stored in the storage.</typeparam>
    public interface IStreamMapper<TItem>
    {
        /// <summary>
        /// Convert <see cref="TItem"/> to a byte array. 
        /// </summary>
        byte[] Convert(TItem item);

        /// <summary>
        /// Convert a byte array to a <see cref="TItem"/>. 
        /// </summary>
        TItem Convert(byte[] buffer);
    }
}
