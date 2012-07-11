using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tiny.Collections
{
    public class SubList<T> : IReadOnlyList<T>, IList<T>
    {
        readonly IReadOnlyList<T> m_wrapped;
        readonly int m_startIndex;
        readonly int m_count;

        public SubList(IReadOnlyList<T> wrapped, int startIndex)
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

        public SubList(IReadOnlyList<T> wrapped, int startIndex, int length)
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = m_startIndex; i < m_startIndex + m_count; ++i) {
                yield return m_wrapped[i];
            }
        }

        public int Count
        {
            get { return m_count; }
        }

        public T this[int index]
        {
            get { return m_wrapped[index.CheckGTE(0, "index").CheckLT(m_count, "index") + m_startIndex]; }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; ++i) {
                array[arrayIndex + i] = this[i];
            }
        }

        //TODO: Finish implementing this
        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public bool IsReadOnly
        {
            get { throw new System.NotImplementedException(); }
        }

        public int IndexOf(T item)
        {
            throw new System.NotImplementedException();
        }

        void ICollection<T>.Add(T item)
        {
            throw new System.NotImplementedException();
        }

        void ICollection<T>.Clear()
        {
            throw new System.NotImplementedException();
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new System.NotImplementedException();
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new System.NotImplementedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        T IList<T>.this[int index]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
