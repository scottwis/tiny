// 
// PEHeader.cs
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

namespace Tiny.Metadata.Layout
{
    //# Defines the layout of the PE Header in a managed executable.
    //# Reference
    //# ===================
    //# * Partion II Section 25.2.2 ECMA 335 Spec, Version 5
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct PEHeader
    {
        //# The set of bits in the image characteristics map that tdc does not support
        private const ImageCharacteristics s_disallowedImageCharacteristics = 
            ImageCharacteristics.IMAGE_FILE_BYTES_REVERSED_LO
            | ImageCharacteristics.IMAGE_FILE_SYSTEM
            | ImageCharacteristics.IMAGE_FILE_UP_SYSTEM_ONLY
            | ImageCharacteristics.IMAGE_FILE_BYTES_REVERSED_HI;
            
        //# The machine targeted by this executable.
        [FieldOffset(0)]
        public readonly MachineType Machine;

        //# The number of sections in an image.
        [FieldOffset(2)]
        public readonly short NumberOfSections;

        //# Date and tieme the file was creaed, in seconds, since Midnight on January 1st 1970.
        [FieldOffset(5)]
        public readonly int TimeStamp;

        //# The pointer (file offset) to the symbol table. This should always be 0 for any executable image
        //# including managed executables. It only should be non-null for object files.
        //#
        //# Reference
        //# ===================
        //# * PE/COFF Spec Version 8.2 § 2.3
        [FieldOffset(8)]
        public readonly int PointerToSymbolTable;

        //# The number of symbols in the symbol table. Again, for executable images this should always be 0.
        [FieldOffset(12)]
        public readonly int NumberOfSymbols;

        //# The number of byes in the "optional" header. For exexecutable images this should always be non 0. For
        //# managed executables supported by the Tiny Decompiler, this should always be >= 224 for 32 bit images and
        //# >= 240 for 64 bit images.
        [FieldOffset(16)]
        public readonly ushort OptionalHeaderSize;

        //# A set of bit flags indicating the attributess of the file.
        [FieldOffset(18)]
        public readonly ImageCharacteristics Characteristics;

        public bool Validate(byte * pFileBase, uint fileSize) {
            fileSize.AssumeGT(0U);
            fixed(PEHeader * pThis = &this) {
                FluidAsserts.Assume(pThis >= pFileBase && pThis < checked(pFileBase + fileSize));
                FluidAsserts.Assume(fileSize - ((byte *)pThis - pFileBase) >= sizeof(PEHeader));

                try {
                    //NOTE: This code could be made more consise using compound conditionals, but this orginization makes
                    //the code much easier to debug.

                    //1. There should be a null symbol table pointer (Partion II, Section 25.2.2 ECMA-335 V 5).
                    if (PointerToSymbolTable != 0) {
                        return false;
                    }

                    //2. Therefore, there should be 0 symbols (Partion II, Section 25.2.2 ECMA-335 V 5).
                    if (NumberOfSymbols != 0) {
                        return false;
                    }

                    //3. An optional header should be present, and it size should be at least 224 BYTES.
                    //On 64 bit systems it should be at least 240 bytes, but we won't check that until
                    //we identify the image type when we process the "Optional Header".
                    if (OptionalHeaderSize < 224) {
                        return false;
                    }

                    //4. The optional header should be within the bounds of the file.

                    if (checked(((byte *)pThis + sizeof(PEHeader) + OptionalHeaderSize) >= pFileBase + fileSize)) {
                        return false;
                    }

                    //5. Is the machine type equal to IMAGE_FILE_MACHINE_UNKNOWN, IMAGE_FILE_MACHINE_I386 or IMAGE_FILE_MACHINE_AMD64?
                    if (! (
                        Machine == MachineType.IMAGE_FILE_MACHINE_UNKNOWN
                        || Machine == MachineType.IMAGE_FILE_MACHINE_I386
                        || Machine == MachineType.IMAGE_FILE_MACHINE_AMD64
                    )) {
                        return false;
                    }

                    //6. Has the image been marked by a linker as being "complete"
                    if (
                        (Characteristics & ImageCharacteristics.IMAGE_FILE_EXECUTABLE_IMAGE)
                        != ImageCharacteristics.IMAGE_FILE_EXECUTABLE_IMAGE
                    ) {
                        return false;
                    }

                    //7. Are all unsupported flags set to 0? We are overly conservative in the images we accept. If
                    //   something looks like it does not belong in a managed executable we reject it, even if we could
                    //   get away with ignoring it. We should only allow these if real cases come up that require them.
                    if ((Characteristics & s_disallowedImageCharacteristics) != 0) {
                        return false;
                    }

                    return true;
                }
                catch (OverflowException) {
                    return false;
                }
            }
        }
    }
}

