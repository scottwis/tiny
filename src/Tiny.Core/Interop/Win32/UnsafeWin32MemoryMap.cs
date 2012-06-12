// UnsafeWin32MemoryMap.cs
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

#if WIN32
using System;

namespace Tiny.Interop.Win32
{
    public sealed unsafe class UnsafeWin32MemoryMap : IUnsafeMemoryMap
    {
        IntPtr m_fileHandle;
        IntPtr m_mapHandle;
        void* m_pData;
        readonly uint m_size;

        public UnsafeWin32MemoryMap(IntPtr hFile, IntPtr hMapping, void* pData, uint size)
        {
            m_fileHandle = hFile.AssumeNotNull();
            m_mapHandle = hMapping.AssumeNotNull();
            m_pData = FluidAsserts.AssumeNotNull(pData);
            m_size = size.AssumeGTZ();
        }

        ~UnsafeWin32MemoryMap()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_pData != null) {
                Win32Platform.UnmapViewOfFile(m_pData);
                m_pData = null;
            }

            if (m_mapHandle != IntPtr.Zero) {
                Win32Platform.CloseHandle(m_mapHandle);
                m_mapHandle = IntPtr.Zero;
            }

            if (m_fileHandle != IntPtr.Zero) {
                Win32Platform.CloseHandle(m_fileHandle);
                m_fileHandle = IntPtr.Zero;
            }
        }

        public void* Data
        {
            get { return m_pData; }
        }

        public uint Size
        {
            get { return m_size; }
        }
    }
}

#endif