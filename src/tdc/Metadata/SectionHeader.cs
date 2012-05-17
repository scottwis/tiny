// 
// SectionHeader.cs
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
using System;
using System.Runtime.InteropServices;

namespace Tiny.Decompiler.Metadata
{
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct SectionHeader
    {
        [FieldOffset(0)]
        public fixed byte Name[8];

        [FieldOffset(8)]
        public readonly uint VirtualSize;

        [FieldOffset(12)]
        public readonly uint VirtualAddress;

        [FieldOffset(16)]
        public readonly uint SizeOfRawData;

        [FieldOffset(20)]
        public readonly uint PointerToRawData;

        [FieldOffset(24)]
        public readonly uint PointerToRelocations;

        [FieldOffset(28)]
        public readonly uint PointerToLineNumbers;

        [FieldOffset(32)]
        public readonly ushort NumberOfRelocations;

        [FieldOffset(34)]
        public readonly ushort NumberOfLineNumbers;

        [FieldOffset(36)]
        public SectionCharacteristics Characteristics;

        public bool Verify()
        {
            #error "Implement this"
        }
    }
}

