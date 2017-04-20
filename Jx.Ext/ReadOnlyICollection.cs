using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jx.Ext
{
    public sealed class ReadOnlyICollection<T> : ICollection<T>, IEnumerable<T>, System.Collections.IEnumerable
    {
        private ICollection<T> innerCollection;

        public int Count
        {
            get
            {
                return this.innerCollection.Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }
        public ReadOnlyICollection(ICollection<T> collection)
        {
            this.innerCollection = collection;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.innerCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() 
        {
            return this.innerCollection.GetEnumerator();
        }

        public bool Contains(T item)
        {
            return this.innerCollection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.innerCollection.CopyTo(array, arrayIndex);
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }
    }
}
