// 
// FileFormat.cs
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

namespace Tiny.Decompiler.Metadata
{
    //# Defines an enum encapsulating the valid values for the "Magic number" field preceeding the optional header in
    //# a PE/COFF file.
    //#
    //# Reference: PE/COFF spec version 8.2 p 17
    enum FileFormat : short
    {
        //# A 32 bit executable image.
        PE32 = 0x10b,
        //# A an extension to PE32 that defines 64 bit RVA (relative virtual address) fields instead of the 32 bit
        //# values used in PE32. It is used for 64 bit executables. .
        PE32_PLUS = 0x20b,
        //# This value is called out in the PE/COFF spec, so it is included here for completeness.
        //# It is not supported by tdc.
        ROM_IMAGE = 0x107
    }
}

