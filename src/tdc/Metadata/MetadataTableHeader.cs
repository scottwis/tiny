// MetadataTableHeader.cs
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
using System.Runtime.InteropServices;

namespace Tiny.Decompiler.Metadata
{
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct MetadataTableHeader
    {
        [FieldOffset(0)]
        public readonly uint Reserved;

        [FieldOffset(4)]
        public readonly byte MajorVersion;

        [FieldOffset(5)]
        public readonly byte MinorVersion;

        [FieldOffset(6)]
        public readonly byte HeapSizes;

        [FieldOffset(7)]
        public readonly byte Reserved2;

        [FieldOffset(8)]
        public readonly ulong ValidTables;

        [FieldOffset(16)]
        public readonly ulong SortedTables;


        public uint GetRowCount(MetadataTable table)
        {
            if ((byte)table >= (sizeof(ulong)*8)) {
                throw new InvalidOperationException("Invalid meta-data table.");
            }
            if (((1ul << (int)table) & ValidTables) == 0) {
                return 0;
            }

            fixed (MetadataTableHeader* pThis = &this) {
                var pRows = (uint *)((byte*) pThis + 20);
                if (table == 0) {
                    return pRows[0];
                }
                //The index of the row n is the number of 1 bits that preceed position n in the 
                //valid tables bit mask. So we mask out all bits at or above position n in the valid mask
                //and then compute the apropriate count.
                return pRows[(((1ul << (int) table) - 1) & ValidTables).BitCount()];
            }
        }

        public uint Size
        {
            get { return checked((uint)(24u + 4u*ValidTables.BitCount())); }
        }

        public uint GetHeapIndexSize(StreamID id)
        {
            switch (id) {
                case StreamID.Strings:
                    return (HeapSizes & 0x1) != 0 ? 4u : 2u;
                case StreamID.Guid:
                    return (HeapSizes & 0x2) != 0 ? 4u : 2u;
                case StreamID.Blob:
                    return (HeapSizes & 0x4) != 0 ? 4u : 2u;
                default:
                    throw new ArgumentOutOfRangeException("id","Not a valid heap");
            }
        }
    }
}
