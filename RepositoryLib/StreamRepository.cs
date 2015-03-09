// <copyright company="XATA">
//      Copyright (c) 2015, All Right Reserved
// </copyright>
// <author>Ivan Ivchenko</author>
// <email>iivchenko@live.com</email>

using System.Collections.Generic;

namespace RepositoryLib
{
    public class StreamRepository<TItem, TId> : IRepository<TItem, TId>
    {
        public uint Count { get; private set; }

        public IEnumerable<TItem> All { get; private set; }
        
        public TItem GetById(TId id)
        {
            throw new System.NotImplementedException();
        }

        public TId Add(TItem item)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(TItem item)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(TId id)
        {
            throw new System.NotImplementedException();
        }

        public void Update(TItem oldItem, TItem newItem)
        {
            throw new System.NotImplementedException();
        }

        public void Update(TId id, TItem item)
        {
            throw new System.NotImplementedException();
        }
    }
}
