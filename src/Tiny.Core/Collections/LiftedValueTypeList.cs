﻿// LiftedList.cs
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
using Tiny;

namespace Tiny.Collections
{
    static unsafe class LiftedValueTypeList
    {
        public static LiftedValueTypeList<T> Create<T>(
            int itemCount,
            GetRowDelegate rowFectcher,
            CreateObjectDelegate<T> factory, 
            Func<bool> disposedChecker
        ) where T : struct
        {
            return new LiftedValueTypeList<T>(itemCount, rowFectcher, factory, disposedChecker);
        }
    }

    public sealed unsafe class LiftedValueTypeList<T> : ReadonlyListBase<T> where T : struct
    {
        // ReSharper disable InconsistentNaming
        readonly GetRowDelegate FetchRow;
        readonly CreateObjectDelegate<T> CreateObject;
        readonly Func<bool> IsDisposed;
        // ReSharper restore InconsistentNaming

        readonly int m_itemCount;

        internal LiftedValueTypeList(
            int itemCount,
            GetRowDelegate rowFectcher,
            CreateObjectDelegate<T> factory, 
            Func<bool> disposedChecker
        )
        {
            m_itemCount = itemCount.CheckGTE(0, "itemCount");
            FetchRow = rowFectcher.CheckNotNull("rowFetcher");
            CreateObject = factory.CheckNotNull("factory");
            IsDisposed = disposedChecker.CheckNotNull("disposedChecker");
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
                return m_itemCount;
            }
        }

        public override T this[int index]
        {
            get
            {
                CheckDisposed();
                if (index < 0 || index >= Count) {
                    throw new ArgumentOutOfRangeException("index");
                }
                return CreateObject(FetchRow(index));
            }
        }
    }
}
