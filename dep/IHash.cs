// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

namespace Deduplication
{
    /// <summary>
    /// Provides functionality for hash algorithms.
    /// </summary>
    public interface IHash
    {
        /// <summary>
        /// Calculates hash for input data.
        /// </summary>
        /// <param name="data">Input data for which hash must be calculated.</param>
        /// <returns>Hash for input data.</returns>
        byte[] Calculate(byte[] data);
    }
}
