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

namespace Tiny.Decompiler.Metadata.Layout
{
    //# Defines the layout of the "header" at the begining of the "#~"(metadata tables) stream in a managed PE file.
    //# Reference: ECMA-335, 5th Edition, Partition II § 24.2.6
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct MetadataTableHeader
    {
        //# Reserved. Should be 0. The tiny decompiler will conservatively reject any image with a value != 0.
        [FieldOffset(0)]
        public readonly uint Reserved;

        //# The major version of the table schema. The tiny decompiler will conservatively reject any image with a
        //# value != 2.
        [FieldOffset(4)]
        public readonly byte MajorVersion;

        //# The minor version of the table schema. The tiny decompiler will conservatively reject any image with a
        //# value != 0.
        [FieldOffset(5)]
        public readonly byte MinorVersion;

        //# A bit vector of heap sizes. For convience, this can be read using [GetHeapIndexSize].
        [FieldOffset(6)]
        public readonly byte HeapSizes;

        //# Reserved. Should always be 1. The tiny decompiler will conservatively reject any image with a value != 1.
        [FieldOffset(7)]
        public readonly byte Reserved2;

        //# A bit mask of the tables that follow the header. Bits are indexed using values in the [MetadataTable] enum.
        //# If a corresponding bit is 1, then it is present in the image. Otherwise it is not.
        [FieldOffset(8)]
        public readonly ulong ValidTables;

        //# A bit mask indicating which tables are sorted. Like [ValidTables] bits are indexed using values in the
        //# [MetadataTable] enum. The tiny decompiler will reject any image with a bit set in [SortedTables] that is
        //# not also set in [ValidTables]. 
        [FieldOffset(16)]
        public readonly ulong SortedTables;

        //# Returns the number of tables that are present in the meta-data.
        public int NumberOfTables
        {
            get { return ValidTables.BitCount(); }
        }

        //# Returns the number of rows for the given meta-data table. If a table is not present, 0 will be returned.
        //# throws: ArgumentOutOfRangeException - If [table] is not valid.
        public uint GetRowCount(MetadataTable table)
        {
            if ((byte)table >= (sizeof(ulong)*8)) {
                throw new ArgumentOutOfRangeException("table", "Invalid meta-data table.");
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
                //and then compute it's bit count.
                return pRows[(((1ul << (int) table) - 1) & ValidTables).BitCount()];
            }
        }

        //# Returns the total size of the header, in bytes.
        public uint Size
        {
            get { return checked((uint)(24u + 4u*NumberOfTables)); }
        }

        //# Returns the number of bytes used to incode indexes into the given stream.
        //# If the number of entries in the provided stream is <= 2^16, this will be 2. Otherwise 4 will be returned.
        //# throws: ArgumentOutOfRangeException if the provided stream is not any of the following:
        //#     1. [StreamID.Strings]
        //#     2. [StreamID.Guid]
        //#     3. [StreamID.Blob]
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

        public bool Verify()
        {
            //We conservatively validate that Reserved fields have their expected values.
            if (Reserved != 0) {
                return false;
            }

            //We reject anything with a schema number we don't support.
            if (MajorVersion != 2) {
                return false;
            }

            if (MinorVersion != 0) {
                return false;
            }

            //No bit other than 0x1, 0x2, 0x4 can be set in the HeapSizes bit mask.
            if ((HeapSizes & 0xF8) != 0) {
                return false;
            }

            //We conservatively validate that Reserved fields have their expected values.
            if (Reserved2 != 1) {
                return false;
            }

            //No bit above position 0x2c can be set in the ValidTables mask.
            if ((~((1UL << (byte)MetadataTable.MAX_TABLE_ID + 1) - 1) & ValidTables) != 0) {
                return false;
            }

            if (NumberOfTables < 1) {
                return false;
            }

            return true;
        }
    }
}
