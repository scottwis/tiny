// ReadonlyListBase.cs
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
using System.Linq;

namespace Tiny.Collections
{
    public abstract class ReadonlyListBase<T> : IReadOnlyList<T>, IList<T>
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract IEnumerator<T> GetEnumerator();
        protected abstract void CheckDisposed();
        public abstract int Count { get; }

        public bool IsReadOnly
        {
            get
            {
                CheckDisposed();
                return true;
            }
        }

        public abstract T this[int index] { get; }

        public void Add(T item)
        {
            CheckDisposed();
            throw new NotSupportedException("The list is readonly.");
        }

        public void Clear()
        {
            CheckDisposed();
            throw new NotSupportedException("The list is readonly.");
        }

        public bool Contains(T item)
        {
            CheckDisposed();
            return this.Any(x => EqualityComparer<T>.Default.Equals(x, item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            CheckDisposed();
            foreach (var item in this) {
                array[arrayIndex++] = item;
            }
        }

        public bool Remove(T item)
        {
            CheckDisposed();
            throw new NotSupportedException("The list is readonly.");
        }

        public int IndexOf(T item)
        {
            CheckDisposed();
            var ret = 0;
            foreach (var x in this) {
                if (EqualityComparer<T>.Default.Equals(x, item)) {
                    return ret;
                }
                ++ret;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            CheckDisposed();
            throw new NotSupportedException("The list is readonly.");
        }

        public void RemoveAt(int index)
        {
            CheckDisposed();
            throw new NotSupportedException("The list is readonly.");
        }

        T IList<T>.this[int index]
        {
            get
            {
                CheckDisposed();
                return this[index];
            }
            set
            {
                CheckDisposed();
                throw new NotSupportedException("The list is readonly.");
            }
        }
    }
}
