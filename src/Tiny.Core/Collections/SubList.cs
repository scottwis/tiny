using System;
using System.Collections;
using System.Collections.Generic;

namespace Tiny.Collections
{
    public class SubList<T> : IReadOnlyList<T>, IList<T>
    {
        readonly IReadOnlyList<T> m_wrapped;
        readonly int m_startIndex;
        readonly int m_count;

        public SubList(IReadOnlyList<T> wrapped, int startIndex)
        {
            m_wrapped = wrapped.CheckNotNull("wrapped");
            if (startIndex > wrapped.Count) {
                startIndex = wrapped.Count;
            }
            m_startIndex = startIndex.CheckGTE(0, "startIndex");
            m_count = wrapped.Count - startIndex;
        }

        public SubList(IReadOnlyList<T> wrapped, int startIndex, int length)
        {
            m_wrapped = wrapped.CheckNotNull("wrapped");
            if (startIndex > wrapped.Count) {
                startIndex = wrapped.Count;
            }
            m_startIndex = startIndex.CheckGTE(0, "startIndex");
            length.CheckGTE(0, "length");
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

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < m_count; ++i) {
                if (EqualityComparer<T>.Default.Equals(this[i], item)) {
                    return i;
                }
            }
            return -1;
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException("The collection is read only.");
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException("The collection is read only.");
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException("The collection is read only.");
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException("The collection is read only.");
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException("The collection is read only.");
        }

        T IList<T>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException("The collection is read only."); }
        }
    }
}
