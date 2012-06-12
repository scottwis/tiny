// 
// SectionHeader.cs
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
using System.Text;
using Tiny.Interop;

namespace Tiny.Metadata.Layout
{
    //TODO: Add doc comments.
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct SectionHeader
    {
        [FieldOffset(0)]
        public fixed byte Name[8];

        [FieldOffset(8)]
        public readonly uint VirtualSize;

        [FieldOffset(12)]
        public readonly uint VirtualAddress;

        [FieldOffset(16)]
        public readonly uint SizeOfRawData;

        [FieldOffset(20)]
        public readonly uint PointerToRawData;

        [FieldOffset(24)]
        public readonly uint PointerToRelocations;

        [FieldOffset(28)]
        public readonly uint PointerToLineNumbers;

        [FieldOffset(32)]
        public readonly ushort NumberOfRelocations;

        [FieldOffset(34)]
        public readonly ushort NumberOfLineNumbers;

        [FieldOffset(36)]
        public SectionCharacteristics Characteristics;

        private string GetNameAsString()
        {

            fixed (byte* pName = Name) {
                var length = NativePlatform.Default.StrLen(pName, 8);
                return new string((sbyte *)pName, 0, length, Encoding.ASCII);
            }
        }

        public bool Verify(OptionalHeader header, byte * pFilePointer, uint fileSize, PEHeader * pPeHeader)
        {
            //1. The name is not empty.
            if (GetNameAsString().Trim() == "") {
                return false;
            }

            //2. VirtualAddress is a multiple of the section alignment.
            if (VirtualAddress == 0 || (VirtualAddress % header.SectionAlignment) != 0) {
                return false;
            }

            //3. SizeOfRawData is a multiple of the file alignment.
            if ((SizeOfRawData % header.FileAlignment) != 0) {
                return false;
            }
            
            //4. PointerToRawData is a multiple of the file alignment
            if ((PointerToRawData % header.FileAlignment) != 0) {
                return false;
            }

            //5. PointerToRawData == 0 <-> SizeOfRawData == 0
            if ((PointerToRawData == 0) != (SizeOfRawData == 0)) {
                return false;
            }

            //6. If PointerToRawData is not null, it must fit inside the file.
            try {
                if (PointerToRawData > fileSize || checked(PointerToRawData + SizeOfRawData) > fileSize) {
                    return false;
                }
            } catch (OverflowException) {
                return false;
            }

            //7. PointerToRelocations must be 0.
            if (PointerToRelocations != 0) {
                return false;
            }

            //8. PointerToLineNumbers must be 0.
            if (PointerToLineNumbers != 0) {
                return false;
            }

            //9. NumberOfRelocations must be 0.
            if (NumberOfRelocations != 0) {
                return false;
            }

            //10. NumberOfLineNumbers must be 0.
            if (NumberOfLineNumbers != 0) {
                return false;
            }

            //11. The characteristics must be valid. We seperate out the check for
            //bits marked as reserved from the check for defined bits that we don't support.
            if ((Characteristics & SectionCharacteristics.Reserved) != 0) {
                return false;
            }

            if ((Characteristics & SectionCharacteristics.DisallowedFlags) != 0) {
                return false;
            }

            return true;
        }

        public uint GetAlignedVirtualSize(OptionalHeader optionalHeader)
        {
            return Util.Pad(VirtualSize, optionalHeader.SectionAlignment);
        }
    }
}

