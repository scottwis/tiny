// CreateFileAttributes.cs
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
namespace Tiny.Interop.Win32
{
    enum CreateFileAttributes : uint
    {
        FILE_ATTRIBUTE_ARCHIVE=0x20,
        FILE_ATTRIBUE_ENCRYPTED=0x4000,
        FILE_ATTRIBUTE_HIDDEN=0x2,
        FILE_ATTRIBUTE_NORMAL=0x80,
        FILE_ATTRIBUTE_OFFLINE=0x1000,
        FILE_ATTRIBUTE_READONLY=0x1,
        FILE_ATTRIBUTE_SYSTEM=0x4,
        FILE_ATTRIBUTE_TEMPORARY=0x100,
        FILE_FLAG_BACKUP_SEMANTICS=0x02000000,
        FILE_FLAG_DELETE_ON_CLOSE=0x04000000,
        FILE_FLAG_NO_BUFFERING=0x20000000,
        FILE_FLAG_OPEN_NO_RECALL=0x00100000,
        FILE_FLAG_OPEN_REPART_POINT=0x00200000,
        FILE_FLAG_OVERLAPPED=0x40000000,
        FILE_FLAG_POSIX_SEMANTICS=0x01000000,
        FILE_FLAG_RANDOM_ACCESS=0x10000000,
        FILE_FLAG_SEQUENTIAL_SCAN=0x08000000,
        FILE_FLAG_WRITE_THROUGH=0x80000000
    }
}
#endif