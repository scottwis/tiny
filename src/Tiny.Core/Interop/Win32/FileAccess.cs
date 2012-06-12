// FileMapAccess.cs
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
    enum FileAccess : uint
    {
        GENERIC_ALL = 0x10000000,
        GENERIC_EXECUTE = 0x20000000,
        GENERIC_READ = 0x80000000,
        GENERIC_WRITE = 0x40000000,
        FILE_GENERIC_EXECUTE = 
            FILE_EXECUTE
            | FILE_READ_ATTRIBUTES
            | STANDARD_RIGHTS_EXECUTE
            | SYNCHRONIZE,
        FILE_GENERIC_READ = 
            FILE_READ_ATTRIBUTES
            | FILE_READ_DATA
            | FILE_READ_EA
            | STANDARD_RIGHTS_READ
            | SYNCHRONIZE,
        FILE_GENERIC_WRITE = 
            FILE_APPEND_DATA
            | FILE_WRITE_ATTRIBUTES
            | FILE_WRITE_DATA
            | FILE_WRITE_EA
            | STANDARD_RIGHTS_WRITE
            | SYNCHRONIZE,
        FILE_ADD_FILE = 0x2,
        FILE_ADD_SUBDIRECTORY = 0x4,
        FILE_ALL_ACCESS = 
            STANDARD_RIGHTS_REQUIRED 
            | SYNCHRONIZE 
            | 511,
        FILE_APPEND_DATA = 0x4,
        FILE_CREATE_PIPE_INSTANCE = 0x4,
        FILE_DELETE_CHILD = 0x40,
        FILE_EXECUTE = 0x20,
        FILE_LIST_DIRECTORY = 0x1,
        FILE_READ_ATTRIBUTES = 0x80,
        FILE_READ_DATA = 0x1,
        FILE_READ_EA = 0x8,
        FILE_TRAVERSE = 0x20,
        FILE_WRITE_ATTRIBUTES = 0x100,
        FILE_WRITE_DATA = 0x2,
        FILE_WRITE_EA = 0x10,
        READ_CONTROL=0x00020000,
        STANDARD_RIGHTS_READ = READ_CONTROL,
        STANDARD_RIGHTS_WRITE = READ_CONTROL,
        STANDARD_RIGHTS_EXECUTE = READ_CONTROL,
        STANDARD_RIGHTS_REQUIRED = 0x000F0000,
        SYNCHRONIZE = 0x00100000
    }
}

#endif