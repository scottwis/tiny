// PEFile.cs
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
using System.IO;
using Tiny.Decompiler.Interop;

namespace Tiny.Decompiler.Metadata
{
    //# Provides a thread-safe, read-only representation of a managed PEFile, loaded via memory-mapped IO.
    sealed unsafe class PEFile : IDisposable
    {
        public const int MSDosStubSize = 128;
        private static readonly int s_peSignature;

        static PEFile()
        {
            if (BitConverter.IsLittleEndian) {
                s_peSignature = (int)(byte)'P' | (((int)(byte)'E') << 8);
            }
            else {
                s_peSignature = (((int)(byte)'P')<< 24) | (((int)(byte)'E') << 16);
            }

        }

        IUnsafeMemoryMap m_memoryMap;
        byte * m_pData;
        uint m_fileSize;

        private PEHeader * m_peHeader;
        private OptionalHeader m_optionalHeader;
        CLRHeader* m_clrHeader;

        public PEFile(String fileName)
        {
            try {
                m_memoryMap = NativePlatform.Default.MemoryMapFile(fileName);
                m_pData = (byte *)m_memoryMap.Data;
                m_fileSize = m_memoryMap.Size;

                if (!(VerifyPEHeader() && VerifyOptionalHeader() && LoadSectionTable() && VerifyCLRHeader())) {
                    throw new FileLoadException("The file is not a valid managed executable.", fileName);
                }
            }
            catch (Exception ex) {
                Dispose();
                if (ex is FileLoadException) {
                    throw;
                }
                throw new FileLoadException("Unable to load assembly", fileName, ex);
            }
        }

        private PEHeader * PEHeader {
            get {
                if (m_peHeader == null) {
                    m_peHeader = (PEHeader*)(PESignature + 1);
                }
                return m_peHeader;
            }
        }

        public void Dispose()
        {
            if (m_memoryMap != null) {
                m_memoryMap.Dispose();
                m_memoryMap = null;
            }
            m_pData = null;
            m_fileSize = 0;
            m_peHeader = null;
            m_optionalHeader = null;
            m_clrHeader = null;
        }

        private int * PESignature
        {
            get { return (int *)((*(int *)(m_pData + 0x3c)) + m_pData); }
        }

        private FileFormat FileFormat
        {
            get {
                return *(FileFormat *)(PEHeader + 1);
            }
        }

        private bool VerifyPEHeader()
        {
            //From Partion II, section 25.2.1 of the ECMA-335 spec, Version 5:
            //1. Is the file at least 128 bytes in size?
            if (m_fileSize < 128) {
                return false;
            }

            //2. Is the 4 byte offset located at 0x3C within the bounds of the file,
            //and is there enough room in the file after it to inclue the PE Header?
            if (PESignature < m_pData || PESignature >= (m_pData + m_fileSize - (sizeof(int) + sizeof(PEHeader)))) {
                return false;
            }

            //3. Are the contents at the address pointed to by 0x3c equal to 'P', 'E', '\0', '\0'
            if (*(int *)PESignature != s_peSignature) {
                return false;
            }

            //4. Check to see if the PE Header is valid. This checks a bunch of stuff.
            if (! PEHeader->Validate(m_pData, m_fileSize)) {
                return false;
            }

            //TODO: If the following flag is set in the image characteristics, it must be validated with the CLR header.
            //IMAGE_FILE_32BIT_MACHINE
            return true;
        }

        private bool VerifyOptionalHeader()
        {
            if (FileFormat == FileFormat.PE32) {
                //I admit, there is some crazy imperitiveness going on here...
                //So, if you don't quite follow the logic, I appologize.
                //BUT, byt the time this function executes we should have verified that the
                //OptionalHeader is large enough to handle the 32bit image type, so we can
                //just go ahead and create the optioanl header 32.
                m_optionalHeader = new OptionalHeader32((OptionalHeaderLayout32 *)(PEHeader + 1));
            }
            else if (FileFormat == FileFormat.PE32_PLUS) {
                //For 64 bit images, the optional header size needs to be a little bit larger (than it is for 32 bit
                //images), but we couldn't know we were a 64 bit image until we examined the "magic number" (file
                //format) in the OptionalHeader which we do after valiating the PE Header (because it's logically not
                //part of the PE header, but is documented as part of the optional header. We don't read the optional
                //header (which succeeds the PEHeader) until we know the PEHeader is valid, which at a minimum
                //means there must be 224 bytes in the optional header. If the PEHEader is not valid, then we can't
                //read the Optional Header, because it might not exist. In any case, once we know we can reade the
                //file format, we do. If it turns out we have a 64 bit image, then we have to do another length check
                //on the OptionalHeaderSize here. Note that we would have already verified the optional header
                //size against the file size, so we don't need to repeat that check.
                if (PEHeader->OptionalHeaderSize < 240) {
                    return false;
                }
                m_optionalHeader = new OptionalHeader64((OptionalHeaderLayout64 *)(PEHeader + 1));
            }
            else {
                return false;
            }

            return m_optionalHeader.Verify(m_fileSize, PEHeader);
        }

        private SectionHeader * FindSection(uint virtualAddress)
        {
            if (PEHeader->NumberOfSections == 0) {
                return null;
            }
            //1. Find the greatest lower bound of virtualAddress in the section table.
            var first = SectionTable;
            var min = SectionTable;
            var max = SectionTable + PEHeader->NumberOfSections - 1;
            while (max >= first && max != min) {
                var mid = (((max - min) + 1) / 2) + min;
                if (mid->VirtualAddress == virtualAddress) {
                    return mid;
                }
                else if (virtualAddress < mid->VirtualAddress) {
                    max = mid - 1;
                }
                else {
                    virtualAddress.AssumeGT(mid->VirtualAddress);
                    min = mid;
                }
            }
            if (max < first) {
                return null;
            }

            //2. Verify that the virtual address fits within the found section
            //(if we select the last section as the glb, the virtual address might be outside it).
            try {
                virtualAddress.AssumeGTE(min->VirtualAddress);
                if (virtualAddress > checked(min->VirtualAddress + min->VirtualSize)) {
                    return null;
                }
            }
            catch (OverflowException) {
                return null;
            }
            return min;
        }

        private bool LoadSectionTable()
        {
            SectionHeader* pLast = null;
            var pCurrent = SectionTable;
            for (int i = 0; i < PEHeader->NumberOfSections; ++i, ++pCurrent) {
                try {
                    if (!pCurrent->Verify(m_optionalHeader, m_pData, m_fileSize, PEHeader))     {
                        return false;
                    }
                    if (pLast != null) {
                        try {
                            //Per the PE/COFF Spec Version 8.2, § 3:
                            //    > In an image file, the VAs for sections must be assigned by the linker so that they
                            //    > are in ascending order and adjacent, and they must be a multiple of the 
                            //    > SectionAlignment value in the optional header.
                            //We check the alignment inside the call to Verify(). We check the 
                            //"ascending order" / adjacney here.
                            if (
                                pCurrent->VirtualAddress < pLast->VirtualAddress 
                                || checked(pLast->VirtualAddress + pLast->GetAlignedVirtualSize(m_optionalHeader)) != pCurrent->VirtualAddress
                            ) {
                                return false;
                            }
                        }
                        catch (OverflowException) {
                            return false;
                        }
                    }
                    pLast = pCurrent;
                } catch (InvalidOperationException) {
                    return false;
                }
            }
            return true;
        }

        SectionHeader* SectionTable
        {
            get { return (SectionHeader*)(m_optionalHeader.GetAddress() + PEHeader->OptionalHeaderSize); }
        }

        private bool VerifyCLRHeader()
        {
            //1. Can we find the section containing the CLR header?
            var runtimeHeader = m_optionalHeader.CLRRuntimeHeader;
            var pSection = FindSection(runtimeHeader->RVA);
            if (pSection == null) {
                return false;
            }

            //2. Does the section have initialized data?
            if (pSection->PointerToRawData == 0 || pSection->SizeOfRawData == 0) {
                return false;
            }

            //3. Is the CLR header located entirely within the initialized data portion of the section?
            //Although it is possible that some portion of the header could be placed inside unitialized data,
            //it is unlikely. Such a case is more likely indicative of a malformed image.

            try {
                if (checked(runtimeHeader->RVA - pSection->VirtualAddress) > pSection->SizeOfRawData) {
                    return false;
                }

                if (
                    checked(runtimeHeader->RVA - pSection->VirtualAddress + runtimeHeader->Size) 
                    > pSection->SizeOfRawData
                ) {
                    return false;
                }
            }
            catch(OverflowException) {
                return false;
            }

            m_clrHeader = (CLRHeader *)((m_pData + (runtimeHeader->RVA - pSection->VirtualAddress)) + pSection->PointerToRawData);
            return m_clrHeader->Verify(m_optionalHeader);
        }
    }
}

