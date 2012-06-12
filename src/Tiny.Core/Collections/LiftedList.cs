// LiftedList.cs
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
using System.Collections.Generic;
using System.Threading;
using Tiny;

namespace Tiny.Collections
{
    public unsafe delegate void* GetRowDelegate(uint index);
    public unsafe delegate T CreateObjectDelegate<out T>(void* pRow);

    public sealed unsafe class LiftedList<T> : ReadonlyListBase<T> where T : class
    {
        // ReSharper disable InconsistentNaming
        readonly GetRowDelegate FetchRow;
        readonly CreateObjectDelegate<T> CreateObject;
        readonly Func<bool> IsDisposed;
        // ReSharper restore InconsistentNaming

        readonly T[] m_array;

        public LiftedList(
            uint itemCount,
            GetRowDelegate rowFectcher,
            CreateObjectDelegate<T> factory,
            Func<bool> disposedChecker
        )
        {
            itemCount.CheckGTE(0U, "itemCount");

            FetchRow = rowFectcher.CheckNotNull("rowFetcher");
            CreateObject = factory.CheckNotNull("factory");
            IsDisposed = disposedChecker.CheckNotNull("disposedChecker");

            m_array = new T[itemCount];
        }

        public override IEnumerator<T> GetEnumerator()
        {
            CheckDisposed();
            for (var i = 0; i < Count; ++i) {
                yield return this[i];
            }
        }

        protected override void CheckDisposed()
        {
            if (IsDisposed()) {
                throw new ObjectDisposedException("Collection");
            }
        }

        public override int Count
        {
            get
            {
                CheckDisposed();
                return m_array.Length;
            }
        }

        public override T this[int index]
        {
            get
            {
                CheckDisposed();
                if (index < 0 || index >= m_array.Length) {
                    throw new ArgumentOutOfRangeException("index");
                }
                LoadObject((uint) index);
                return m_array[index];
            }
        }

        void LoadObject(uint index)
        {
            if (m_array[index] == null) {
                var obj = CreateObject(FetchRow(index));
                Interlocked.CompareExchange(ref m_array[index], obj, null);
            }
        }
    }
}
