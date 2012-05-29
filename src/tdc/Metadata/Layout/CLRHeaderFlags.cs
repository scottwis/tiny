// CLRHeaderFlags.cs
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

namespace Tiny.Decompiler.Metadata.Layout
{
    //# An enum defining the set of "known" flags used in the [CLRHeader.Flags] field. 
    //# Reference: ECMA 335 Spec, 5th Edition, Partition II § 25.3.3.1.
    [Flags]
    enum CLRHeaderFlags : uint
    {
        //#The bitwise or of all "known" CLR Header flags. That is, the set of flags identified in the ECMA 335 spec.
        KnownFlags = 
            COMIMAGE_FLAGS_ILONLY 
            | COMIMAGE_FLAGS_32BITREQUIRED 
            | COMIMAGE_FLAGS_STRONGNAMESIGNED 
            | COMIMAGE_FLAGS_NATIVE_ENTRYPOINT 
            | COMIMAGE_FLAGS_TRACKDEBUGDATA,

        //# Indicates that the assembly contains pure managed code and is not a "mixed-mode" assembly. The tiny
        //# decompiler will reject an image with this bit not set.
        COMIMAGE_FLAGS_ILONLY=0x00000001,
        //# Indicates that the image can only be loaded into a 32-bit process.
        COMIMAGE_FLAGS_32BITREQUIRED=0x00000002,
        //# Indicates that the image has a strong name signature.
        COMIMAGE_FLAGS_STRONGNAMESIGNED=0x00000008,
        //# Indicates that [CLRHeader.EntryPointToken] refers to a native function, not a managed one. The tiny
        //# decompiler will reject an image that has this flag set to 0.
        COMIMAGE_FLAGS_NATIVE_ENTRYPOINT=0x00000010,
        //# Not sure what this means. It probably has something to do with ENC. The ECMA spec just says "write a 0 and
        //# ignore on read". It's value will be ignored by TDC.
        COMIMAGE_FLAGS_TRACKDEBUGDATA=0x00010000
    }
}
