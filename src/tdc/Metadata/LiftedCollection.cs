using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Tiny.Decompiler.Metadata
{
    unsafe delegate void* GetRowDelegate(uint index);
    unsafe delegate T CreateObjectDelegate<out T>(void* pRow);
    
    sealed unsafe class LiftedCollection<T> : IReadOnlyList<T> where T : class
    {
        readonly GetRowDelegate FetchRow;
        readonly CreateObjectDelegate<T> CreateObject;
        readonly Func<bool> IsDisposed;
        readonly T[] m_array;

        public LiftedCollection(uint itemCount, GetRowDelegate rowFectcher, CreateObjectDelegate<T> factory, Func<bool> disposedChecker)
        {
            FetchRow = rowFectcher.CheckNotNull("rowFetcher");
            CreateObject = factory.CheckNotNull("factory");
            IsDisposed = disposedChecker.CheckNotNull("disposedChecker");

            m_array = new T[itemCount];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            CheckDisposed();
            for (int i = 0; i < Count; ++i) {
                yield return this[i];
            }
        }

        void CheckDisposed()
        {
            if (IsDisposed()) {
                throw new ObjectDisposedException("Collection");
            }
        }

        public int Count
        {
            get
            {
                CheckDisposed();
                return m_array.Length;
            }
        }

        public T this[int index]
        {
            get
            {
                CheckDisposed();
                if (index < 0 || index >= m_array.Length) {
                    throw new IndexOutOfRangeException();
                }
                LoadObject((uint)index);
                return m_array[index];
            }
        }

        private void LoadObject(uint index)
        {
            if (m_array[index] == null) {
                var obj = CreateObject(FetchRow(index));
                Interlocked.CompareExchange(ref m_array[index], obj, null);
            }
        }
    }
}
