// 
// MachineType.cs
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

namespace Tiny.Metadata.Layout
{
    //# Defines an enum identifying the "target machine" of a PE/COFF file.
    //# Verfy few of these are supported by tdc, but eveyrthing documented in the PE
    //# spec is included here for completeness.
    //#
    //# Reference: See PE/COFF Spec Version 8.2, § 2.3.1 for details.
    enum MachineType : ushort
    {
        //# Indicates that the contents of PE file are applicable to any machine type
        IMAGE_FILE_MACHINE_UNKNOWN = 0x0,
        //# The file is only applicable to the "Matsushita AM33".
        IMAGE_FILE_MACHINE_AM33 = 0x1d3,
        //# The file is only applicable to X64 machines.
        IMAGE_FILE_MACHINE_AMD64 = 0x8664,
        //# The file is only applicable to little endian ARM machines.
        IMAGE_FILE_MACHINE_ARM = 0x1c0,
        //# The file is only applicable to ARMv7 (or higher) machines running in "Thumb mode".
        IMAGE_FILE_MACHINE_ARMV7 = 0x1c4,
        //# The file contains EFI byte code.
        IMAGE_FILE_MACHINE_EBC=0xebc,
        //# The file contains code that runs on I386 or later processors
        IMAGE_FILE_MACHINE_I386=0x14c,
        //# The file targets the Itanium family of processors.
        IMAGE_FILE_MACHINE_IA64=0x200,
        //# The file tagets Mitsubishi M32R little endian machines.
        IMAGE_FILE_MACHINE_M32R=0x9041,
        //# The file targets MIPS16 processors
        IMAGE_FILE_MACHINE_MIPS16=0x266,
        //# The file targets a MIPS procrssor with an FPU
        IMAGE_FILE_MACHINE_MIPSFPU=0x366,
        //# The file targets a MIPS16 processor with an FPU
        IMAGE_FILE_MACHINE_MIPSFPU16=0x466,
        //# The file targets a PPC liitle endian machine.
        IMAGE_FILE_MACHINE_POWERPC=0x1f0,
        //# The file targets a PPC machine with an FPU
        IMAGE_FILE_MACHINE_POWERPCFP=0x1f1,
        //# The file targets the MIPS R4000 chip.
        IMAGE_FILE_MACHINE_R4000=0x166,
        //# The file targets an Hitachi SH3
        IMAGE_FILE_MACHINE_SH3=0x1a2,
        //# The file targets an Hitachi SH3 DSP
        IMAGE_FILE_MACHINE_SH3DSP=0x1a3,
        //# The file targets an Hitach SH4
        IMAGE_FILE_MACHINE_SH4=0x1a6,
        //# The file targts an Hitach SH5
        IMAGE_FILE_MACHINE_SH5=0x1a8,
        //# The file contains ARM and Thumb "interworking" code.
        IMAGE_FILE_MACHINE_THUMB=0x1c2,
        //# The file targets MIPS little-endian WCE v2
        IMAGE_FILE_MACHINE_WCEMIPSV2=0x169
    }
}

