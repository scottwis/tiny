// StreamHeader.cs
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

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tiny.Decompiler.Interop;

namespace Tiny.Decompiler.Metadata
{
    //# The memory layout of a "StreamHeader" in a managed PE file.
    //# Reference: ECMA-335, 5th Edition, Partition II, § 24.2.2
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct StreamHeader
    {
        public readonly static IDictionary<string, StreamID> StreamNames = new Dictionary<string, StreamID>() {
            {"#Strings", StreamID.Strings},
            {"#US", StreamID.UserStrings},
            {"#Blog", StreamID.Blob},
            {"#GUID", StreamID.Guid},
            {"#~", StreamID.MetadataTables}
        }.AsReadOnly();

        //# The memory offset to the start of the stream from the meta-data root.
        [FieldOffset(0)]
        public readonly uint Offset;

        //# Size of the stream in bytes. Must be a multiple of 4.
        [FieldOffset(4)]
        public readonly uint Size;

        //# The name of the stream as a null-terminated variable length array of ASCII characters, zero-padded ('\0')
        //# to the next 4-byte boundary. The name is limited to 32 characters.
        public byte * Name
        {
            get
            {
                fixed (uint * pSize = &Size) {
                    return (byte*) (pSize + 1);
                }
            }
        }

        //# Fetches the name of the stream as a managed string. A new string will be allocated each time this method 
        //# is called.
        public string GetNameAsString()
        {
            var length = NativePlatform.Default.StrLen(Name, 32);
            return new string((sbyte *)Name, 0, length);
        }
    }
}
