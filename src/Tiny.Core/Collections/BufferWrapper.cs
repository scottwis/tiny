using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tiny.Collections
{
    internal unsafe sealed class BufferWrapper : IReadOnlyList<byte>, IList<byte>
    {
        readonly byte* m_pBuffer;
        readonly int m_length;

        public BufferWrapper(byte * pBuffer, int length)
        {
            m_pBuffer = (byte*) FluentAsserts.AssumeNotNull((void*) pBuffer, "pBuffer");
            m_length = length.CheckGTE(0, "length");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<byte> GetEnumerator()
        {
            for (var i = 0; i < m_length; ++i) {
                yield return this[i];
            }
        }

        public int Count
        {
            get { return m_length; }
        }

        public byte this[int index]
        {
            get
            {
                index.CheckGTE(0, "index").CheckLT(m_length, "index");
                return m_pBuffer[index];
            }
        }

        public bool Contains(byte item)
        {
            return this.Any(b => b == item);
        }

        public void CopyTo(byte[] array, int arrayIndex)
        {
            for (int i = 0; i < m_length; ++i) {
                array[arrayIndex + i] = this[i];
            }
        }

        public int IndexOf(byte item)
        {
            for (var i = 0; i < m_length; ++i) {
                if (this[i] == item) {
                    return i;
                }
            }
            return -1;
        }

        byte IList<byte>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException("The collection is read only."); }
        }

        void ICollection<byte>.Add(byte item)
        {
            throw new NotSupportedException("The collection is read only."); 
        }

        void ICollection<byte>.Clear()
        {
            throw new NotSupportedException("The collection is read only."); 
        }

        bool ICollection<byte>.Remove(byte item)
        {
            throw new NotSupportedException("The collection is read only."); 
        }

        void IList<byte>.Insert(int index, byte item)
        {
            throw new NotSupportedException("The collection is read only."); 
        }

        void IList<byte>.RemoveAt(int index)
        {
            throw new NotSupportedException("The collection is read only."); 
        }

        public bool IsReadOnly
        {
            get { return true; }
        }
    }
}
