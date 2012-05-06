// 
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

namespace Tiny.Decompiler.Metadata
{
    sealed unsafe class PEFile : IDisposable
    {
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

        byte * m_pData;
        int m_fileSize;
        private PEHeader * m_peHeader;

        private PEHeader * PEHeader {
            get {
                return m_peHeader ?? (m_peHeader = (PEHeader *)(PESignature + 1));
            }
        }

        public PEFile(String fileName)
        {
            m_pData = NativePlatform.MapFile(fileName, out m_fileSize);
            if (! (VerifyPEHeader() && VerifyOptionalHeader())) {
                throw new ArgumentException("The provided file is not a valid managed executable.");
            }
        }

        ~PEFile()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_pData != null) {
                NativePlatform.Default.UnmapFile(m_pData);
                m_pData = null;
            }
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
            if (! PEHeader->Vaidate(m_pData, m_fileSize)) {
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
                m_optionalHeader = new OptionalHeader32((byte *)(PEHeader + 1));
            }
            else if (FileFormat == FileFormat.PE32_PLUS) {
                //For 64 bit images, the optional header size needs to be a little bit larger (than it is for 32 bit
                //images), but we couldn't know we were a 64 bit image until we examined the "magic number" (file
                //format) in the OptionalHeader which we do after valiating the PE Header (because it's logically not
                //part of the PE header, but isdocumented as part of the optional header. We don't read the optional
                //header (which succeeds the PEHeader) until we know the PEHeader is valid, which at a minimum
                //means there must be 224 bytes in the optional header. If the PEHEader is not valid, then we can't
                //read the Optional Header, because it might not exist. In any case, once we know we can reader the
                //file format, we do. If it turns out we have a 64 bit image, then we have to do another length check
                //on the OptionalHeaderSize here. Note that we would have already verified the optional header
                //size against the file size, so we don't need to repeat that check.
                if (PEHeader->OptionalHeaderSize < 240) {
                    return false;
                }
                m_optionalHeader = new OptionalHeader64((byte *)(PEHeader + 1));
            }
        }
    }
}

