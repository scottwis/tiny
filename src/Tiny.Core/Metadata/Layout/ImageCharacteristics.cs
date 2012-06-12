// 
// ImageCharacteristics.cs
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

namespace Tiny.Metadata.Layout
{
    //# Defines an enum for the "image characteristics" bit mask in the PEHeader
    //# structure of a PE image.
    //#
    //# Reference: PE/COFF Spec, Version 8.2 § 2.3.2
    [Flags]
    enum ImageCharacteristics : ushort
    {
        //# Indicates that a file does not contain base relocations and must therefore be loaded at its
        //# preferred base address. If the base address is not available, the loader reports an error.
        //# The default behavior of the linker is to strip base relocations from executable (EXE) files. For managed
        //# executables this will usually be 0.
        IMAGE_FILE_RELOCS_STRIPPED= 0x0001,
        //# Indicates that an image file is valid and can be run. If the flag is not set, it indicates a linker error.
        //# The tiny decompiler will reject an image that does not have this bit set to 1.
        IMAGE_FILE_EXECUTABLE_IMAGE=0x0002,
        //# Indicates that COFF line numbers have been removed from the image. The flag is deprecated, and relates to
        //# an obselete native debugging format that is not used for managed executables. The tiny decompiler will
        //# ignore this flag.
        IMAGE_FILE_LINE_NUMS_STRIPPED=0x0004,
        //# Indicates that COFF symbol table entries for local symbols have been removed from the image. This flag is
        //# deprecated and relates to an obsolete native debugging format that is not used for managed
        //# executables. The tiny decompiler will conservatively reject any image that does not have this bit set to 0.
        IMAGE_FILE_LOCAL_SYMS_STRIPPED=0x0008,
        //# Instruct the pre-windows 2000 loader to aggressively trim working set. This flag is deprecated for
        //# Windows 2000 and later. The tiny decompiler does not care what value this bit is set to.
        IMAGE_FILE_AGGRESSIVE_WS_TRIM=0x0010,
        //# Indicates that the application can handle > 2GB addresses.
        //# The tiny decompiler will ignore this value of this bit.
        IMAGE_FILE_LARGE_ADDRESS_AWARE=0x0020,
        //# This bit is reserved for future use. The tiny decompiler will ignore the value of this bit.
        IMAGE_FILE_RESERVED_1 = 0x0040,
        //# The PE/COFF spec (version 8.2) only says:
        //# > Little endian: the least significant bit (LSB) precedes the most significant bit (MSB) in memory. This
        //# > flag is deprecated and should be zero.
        //#
        //# It does not indicate wether that means the program can only run on little endian systems, or that the
        //# contents of the file are little endian. In either case, it is deprecated and should not be set to 1. The
        //# tiny decompiler will conservatively reject any executable image with this bit set to 1.
        IMAGE_FILE_BYTES_REVERSED_LO=0x0080,
        //# Indicates that the image only runs on 32 bit machines. Per the terms of the ECMA 335 spec, this bit should
        //# only be set to 1 in a managed executable if the ClrRuntimeFlags.COMIMAGE_FLAGS_32BITREQUIRED bit is set to
        //# 1 in the image's CLR header. The tiny decompiler will reject any executable that has this bit set and
        //# does not have the corresponding bit set in the CLR header.
        IMAGE_FILE_32BIT_MACHINE=0x0100,
        //# Indicates that debugging information has been stripped from the file. The tiny decompiler ignores the value
        //# of this bit.
        IMAGE_FILE_DEBUG_STRIPPED=0x0200,
        //# Tells the Windows Loader to fully load and copy the image to the swap file, if it is located on removable
        //# media. The tiny decompiler ignores the value of this bit.
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP=0x0400,
        //# Tells the Windows Loader to fully load the image and copy it to the swap file if it is located on network
        //# media. The tiny decompiler ignores the value of this bit.
        //# </summary>
        IMAGE_FILE_NET_RUN_FROM_SWAP=0x0800,
        //# Indicates that the file is a system file and is not a user program. The tiny decompiler will conservatively
        //# reject any image that has the value of this bit set to 1.
        IMAGE_FILE_SYSTEM=0x1000,
        //# Indicates that the image is a dll. The tiny decompiler ignores the value of this bit.
        IMAGE_FILE_DLL=0x2000,
        //# Indicates that the file should be run only on a uniprocessor machine. The tiny decompiler will
        //# conservatively reject any file that has this bit set to 1.
        IMAGE_FILE_UP_SYSTEM_ONLY=0x4000,
        //# The PE/COFF spec (version 8.2) only says:
        //# > Big endian: the MSB precedes the LSB in memory. This flag is deprecated and should be zero.
        //#
        //# It does not indicate wether that mean the program can only run on big endian systems, or that the
        //# contents of the file are big endian. In either case, it is deprecated and should not be set to 1. The
        //# tiny decompiler will conservatively reject any executable image with this bit set to 1.
        IMAGE_FILE_BYTES_REVERSED_HI=0x8000
    }
}

