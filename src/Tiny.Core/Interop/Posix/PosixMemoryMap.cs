// PosixMemoryMap.cs
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

#if linux

using Mono.Unix.Native;
using System;

namespace Tiny.Interop.Posix
{
    sealed unsafe class PosixMemoryMap : IUnsafeMemoryMap
    {
        void * m_pMemory;
        readonly uint m_size;

        public PosixMemoryMap(void* pMemory, uint size)
        {
            m_pMemory = FluentAsserts.CheckNotNull(pMemory, "pMemory");
            m_size = size.CheckGT(0U, "size");
        }
        public void Dispose()
        {
            if (m_pMemory != null) {
                Syscall.munmap((IntPtr)m_pMemory,m_size);
            }
            m_pMemory = null;
        }

        public unsafe void* Data
        {
            get
            {
                CheckDisposed();
                return m_pMemory;
            }
        }

        public unsafe uint Size
        {
            get
            {
                CheckDisposed();
                return m_size;
            }
        }

        private void CheckDisposed()
        {
            if (m_pMemory == null) {
                throw new ObjectDisposedException("PosixMemoryMap");
            }
        }
    }
}

#endif
