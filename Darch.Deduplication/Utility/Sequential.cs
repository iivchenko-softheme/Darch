// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System;
using System.Collections.Generic;

namespace Darch.Deduplication.Utility
{
    public static class Sequential
    {
        public static void For(int fromInclusive, int toIExclusive, Action<int> body)
        {
            for (var i = fromInclusive; i < toIExclusive; i++)
            {
                body(i);
            }
        }

        public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
        {
            foreach (var item in source)
            {
                body(item);
            }
        }
    }
}
