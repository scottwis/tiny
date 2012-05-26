// MetadataRoot.cs
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
using Tiny.Decompiler.Interop;

namespace Tiny.Decompiler.Metadata
{
    //# Defines the layout of the "Metadata root" header in a managed PE/COFF file.
    //# Reference: ECMA-335, 5th Edition, Partition II, § 24.2.1
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct MetadataRoot
    {
        public static uint MinSize = 20;

        //# The magic signature for CLR metadata. Must be 0x424A5342
        [FieldOffset(0)]
        public readonly uint Signature;

        //# Ignored.
        [FieldOffset(4)]
        public readonly ushort MajorVersion;

        //# Ignored.
        [FieldOffset(6)]
        public readonly ushort MinorVersion;

        //# Reserved. Must be zero. The tiny decompiler will reject any image where this field is not 0.
        [FieldOffset(8)]
        public readonly uint Reserved;

        //# The length of the version string, rounded up to the next 4 byte boundary.
        [FieldOffset(12)]
        public readonly uint Length;

        public string GetVersion()
        {
            if (Length > 255) {
                throw new InvalidOperationException("The version string has an invalid length and cannot be read.");
            }
            fixed  (MetadataRoot * pThis= &this) {
                return new string(
                    (sbyte *)(pThis + 16),
                    0, 
                    NativePlatform.Default.StrLen((byte *)(pThis + 16), checked((int)Length))
                );
            }
        }

        //# Reserved. Must be zero. The tiny decompiler will reject any image where this field is not 0.
        public ushort Flags
        {
            get
            {
                if (Length > 255) {
                    throw new InvalidOperationException("Invalid meta-data root.");
                }
                fixed (MetadataRoot * pThis = &this) {
                    return *(ushort*) ((byte *)pThis + Length + 16);
                }
            }
        }

        //# The number of stream headers.
        public ushort NumberOfStreams
        {
            get
            {
                if (Length > 255) {
                    throw new InvalidOperationException("Invalid meta-data root.");
                }
                fixed (MetadataRoot * pThis  = &this) {
                    return *(ushort*) ((byte*) pThis + Length + 18);
                }
            }
        }

        //# Returns a pointer to the FirstStreamHeader. StreamHeaders are variable length, so they must be read
        //# sequentally.
        public StreamHeader * FirstStreamHeader
        {
            get
            {
                fixed (MetadataRoot* pThis = &this)
                {
                    return (StreamHeader*)((byte*)pThis + Length + 20);
                }
            }
        }
    }
}
