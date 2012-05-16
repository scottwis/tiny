// 
// SectionCharacteristics.cs
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
    // ReSharper disable InconsistentNaming

    //# A bit field describing the known flags in the [SectionHeader.Characteristics] field.
    //# Reference: PE/COFF Spec, Version 8.2 § 3.1
    [Flags]
    enum SectionCharacteristics : uint
    {
        //# The bitwise or of all bits makred "reserved" by the PE/COFF spec.
        //# The tiny decompiler will conservatively reject any image that has
        //# any of these bits set to 1.
        //# 
        //# Furthermore, the PE/COFF spec also says that the "value" 0 is reserved,
        //# so tdc will also reject any image with a [SectionHeader.Characteristics][Characteristics] value of 0.
        Reserved  = 0xE0517,
        //# Indicates that a section should not be padded to the next boundary. This flag is obsolete and is replaced
        //# by IMAGE_SCN_ALIGN_1BYTES. It is valid only for object files. The tiny decompiler will reject any image
        //# with a section that has this bit set to 1.
        IMAGE_SCN_TYPE_NO_PAD = 0x00000008,
        //# Indicates that a section contains executable code.
        IMAGE_SCN_CNT_CODE	= 0x00000020,
        //# Indciates that a section contains initialized data.
        IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040,
        //# Indicates that a section contains uninitialized data.
        IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080,
        //# Reserved for future use.
        IMAGE_SCN_LNK_OTHER = 0x00000100	,
        //# Indicates that a section contains comments or other information. The .drectve section has this type. It is
        //# valid for object files only. The tiny decompiler will reject any image that has a section with this bit set
        //# to 1.
        IMAGE_SCN_LNK_INFO = 0x00000200,
        //# Indicates that a section will not become part of an image when processed by a linker. It is valid only for
        //# object files. The tiny decompiler will reject any image which contains a section that has this bit set.
        IMAGE_SCN_LNK_REMOVE = 0x00000800,
        //# The section contains COMDAT data. COMDAT sections are only valid for object files. The tiny decompiler will
        //# reject any image which contains a section that has this bit set. See PE/COFF Spec, Version 8.2 § 5.5.6, for
        //# more information on COMDAT sections. 
        IMAGE_SCN_LNK_COMDAT = 0x00001000, 
        //# The section contains data referenced through the global pointer (GP).
        IMAGE_SCN_GPREL = 0x00008000, 
        //# Reserved for future use.
        IMAGE_SCN_MEM_PURGEABLE = 0x00020000,
        //# For ARM machine types, the section contains Thumb code.  Reserved for future use with other machine types.
        IMAGE_SCN_MEM_16BIT = 0x00020000	,
        //# Reserved for future use
        IMAGE_SCN_MEM_LOCKED = 0x00040000,
        //# Reserved for future use.
        IMAGE_SCN_MEM_PRELOAD = 0x00080000,
        //# Align data on a 1-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_1BYTES = 0x00100000,
        //# Align data on a 2-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_2BYTES = 0x00200000,
        //# Align data on a 4-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_4BYTES = 0x00300000,
        //# Align data on an 8-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_8BYTES = 0x00400000,
        //# Align data on a 16-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_16BYTES = 0x00500000,
        //# Align data on a 32-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_32BYTES = 0x00600000,
        //# Align data on a 64-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_64BYTES = 0x00700000,
        //# Align data on a 128-byte boundary. Valid only for object files. The tiy decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_128BYTES	 = 0x00800000	,
        //# Align data on a 256-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_256BYTES	 = 0x00900000,
        //# Align data on a 512-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_512BYTES	 = 0x00A00000,
        //# Align data on a 1024-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_1024BYTES = 0x00B00000,
        //# Align data on a 20484-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_2048BYTES = 0x00C00000,
        //# Align data on a 4096-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_4096BYTES = 0x00D00000,
        //# Align data on an 8192-byte boundary. Valid only for object files. The tiny decompiler will reject any image
        //# that has this bit set to 1.
        IMAGE_SCN_ALIGN_8192BYTES = 0x00E00000,
        //# The section contains extended relocations.
        IMAGE_SCN_LNK_NRELOC_OVFL = 0x01000000,
        //# The section can be discarded as needed.
        IMAGE_SCN_MEM_DISCARDABLE = 0x02000000	,
        //# The section cannot be cached.
        IMAGE_SCN_MEM_NOT_CACHED = 0x04000000,
        //# The section is not pageable.
        IMAGE_SCN_MEM_NOT_PAGED = 0x08000000,
        //# The section can be shared in memory.
        IMAGE_SCN_MEM_SHARED = 0x10000000	,
        //# The section can be executed as code.
        IMAGE_SCN_MEM_EXECUTE = 0x20000000,
        //# The section can be read.
        IMAGE_SCN_MEM_READ = 0x40000000,
        //#The section can be written to.
        IMAGE_SCN_MEM_WRITE = 0x80000000
    }
    // ReSharper restore InconsistentNaming
}

