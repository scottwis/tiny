// ProtectionFlags.cs
//  
// Author:
//       Scott Wisniewski <scott@scottdw2.com>
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
    [Flags]
    enum ProtectionFlags : uint
    {
        PAGE_EXECUTE_READ = 0x20,
        PAGE_EXECUTE_READ_WRITE = 0x40,
        PAGE_EXECUTE_WRITE_COPY = 0x80,
        PAGE_READONLY = 0x02,
        PAGE_READWRITE = 0x04,
        PAGE_WRITECOPY = 0x08,
        SEC_COMMIT = 0x8000000,
        SEC_IMAGE = 0x1000000,
        SEC_IMAGE_NO_EXECUTE = 0x11000000,
        SEC_LARGE_PAGES = 0x80000000,
        SEC_NOCACHE = 0x10000000,
        SEC_RESERVE = 0x4000000,
        SEC_WRITECOMBINE = 0x40000000
    }
}

#endif