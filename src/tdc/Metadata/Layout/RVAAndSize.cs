// 
// RVAAndSize.cs
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

using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Tiny.Decompiler.Metadata.Layout
{
    //# Describes a paired RVA and size used in a PE file. It is used to represent:
    //# 1. Data directories in the optional PE header.
    //# Reference: PE/COFF Spec, Vesion 8.2, § 2.4.3
    //# 2. Pointers in the CLR header
    //# Reference: ECMA-335 Spec, 5th edition, Partition II §25.3.3
    [StructLayout(LayoutKind.Explicit)]
    struct RVAAndSize
    {
        [FieldOffset(0)]
        public readonly uint RVA;
        [FieldOffset(4)]
        public readonly uint Size;

        [Pure]
        public bool IsZero()
        {
            return RVA == 0 && Size == 0;
        }

        [Pure]
        public bool IsConsistent()
        {
            return (RVA == 0) == (Size == 0);
        }
    }
}

