// SubList.cs
//  
// Author:
//     Scott Wisniewski <scott@scottdw2.com>
//  
// Copyright (c) 2012 Scott Wisniewski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//  
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Tiny.Collections
{
    class SubList<T> : ISubList<T>, IList<T>
    {
        [NotNull] readonly IReadOnlyList<T> m_wrapped;
        readonly int m_startIndex;
        readonly int m_count;

        public SubList(IReadOnlyList<T> wrapped, int startIndex)
        {
            startIndex.CheckGTE(0, "startIndex");
            var sl = wrapped.CheckNotNull("wrapped") as ISubList<T>;
            if (sl != null) {
                wrapped = sl.Wrapped.AssumeNotNull();
                startIndex += sl.StartIndex;
            }
            if (startIndex > wrapped.Count) {
                startIndex = wrapped.Count;
            }
            m_wrapped = wrapped;
            m_startIndex = startIndex;
            m_count = wrapped.Count - startIndex;
        }

        public SubList(IReadOnlyList<T> wrapped, int startIndex, int length)
        {
            var sl = wrapped.CheckNotNull("wrapped") as ISubList<T>;
            startIndex.CheckGTE(0, "startIndex");
            length.CheckGTE(0, "length");

            if (sl != null) {
                wrapped = sl.Wrapped;
                startIndex += sl.StartIndex;
            }

            if (startIndex > wrapped.Count) {
                startIndex = wrapped.Count;
            }

            var maxCount = wrapped.Count - startIndex;
            if (length > maxCount) {
                length = maxCount;
            }

            m_wrapped = wrapped;
            m_startIndex = startIndex;
            m_count = length;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = m_startIndex; i < m_startIndex + m_count; ++i) {
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
            for (var i = 0; i < Count; ++i) {
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
            for (var i = 0; i < m_count; ++i) {
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

        IReadOnlyList<T> ISubList<T>.Wrapped
        {
            get { return m_wrapped; }
        }

        int ISubList<T>.StartIndex
        {
            get { return m_startIndex; }
        }
    }
}
