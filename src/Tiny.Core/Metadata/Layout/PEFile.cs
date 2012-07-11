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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Tiny.Collections;
using Tiny.Interop;

namespace Tiny.Metadata.Layout
{
    //# Provides a thread-safe, read-only representation of a managed PEFile, loaded via memory-mapped IO.
    sealed unsafe class PEFile : IDisposable
    {
        //# The size of the MSDosStub in a PE header, in bytes.
        public const int MSDosStubSize = 128;

        //# The magic number used to identify a PE file.
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
        string m_fullPath;

        PEHeader* m_peHeader;
        OptionalHeader m_optionalHeader;
        CLRHeader* m_clrHeader;
        StreamHeader*[] m_streams;
        MetadataRoot* m_metadataRoot;
        MetadataTableHeader* m_metadataTableHeader;
        byte*[] m_tables;
        volatile Module m_module;
        Assembly m_assembly;
        uint[] m_codedIndexSizes;

        public PEFile(Assembly assembly, String fileName)
        {
            try {
                m_assembly = assembly.CheckNotNull("assembly");
                m_fullPath = new FileInfo(fileName).FullName;
                m_memoryMap = NativePlatform.Default.MemoryMapFile(fileName);
                m_pData = (byte *)m_memoryMap.Data;
                m_fileSize = m_memoryMap.Size;

                if (!(Verify())) {
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

        bool Verify()
        {
            //This could be a bit more compact if written using &&, but that makes debugging more difficult,
            //so we split them out into seperate if blocks.
            if (!VerifyPEHeader()) {
                return false;
            }
            
            if (! VerifyOptionalHeader()) {
                return false;
            }
            
            if (!LoadSectionTable()) {
                return false;
            }
            if (!VerifyCLRHeader()) {
                return false;
            } 
            if (!VerifyMetadataRoot()) {
                return false;
            }
            if (!LoadMetadataTables()) {
                return false;
            }
            return true;
        }

        bool LoadMetadataTables()
        {
            var pStreamHeader = (StreamHeader *)m_streams[(int)StreamID.MetadataTables];
            if (pStreamHeader == null) {
                return false;
            }
            m_metadataTableHeader = (MetadataTableHeader *)checked((byte*) m_metadataRoot + pStreamHeader->Offset);
            if (!m_metadataTableHeader->Verify()) {
                return false;
            }
            ComputeCodedIndexSizes();
            m_tables = new byte*[(int)MetadataTable.MAX_TABLE_ID + 1];
            var pCurrent = (byte*) m_metadataTableHeader + m_metadataTableHeader->Size;
            for (MetadataTable i = 0; i <= MetadataTable.MAX_TABLE_ID; ++i ) {
                if (GetRowCount(i) != 0) {
                    m_tables[(int)i] = pCurrent;
                    pCurrent += GetRowSize(i)*GetRowCount(i);
                }
            }
            return true;
        }

        void ComputeCodedIndexSizes()
        {
            m_codedIndexSizes = new uint[(int) CodedIndex.NUMBER_OF_CODED_INDEX_TYPES];
            m_codedIndexSizes[(int)CodedIndex.TypeDefOrRef] = ComputeCodedIndexSize(
                2,
                MetadataTable.TypeDef,
                MetadataTable.TypeRef,
                MetadataTable.TypeSpec
            );
            m_codedIndexSizes[(int)CodedIndex.HasConstant] = ComputeCodedIndexSize(
                2,
                MetadataTable.Field,
                MetadataTable.Param,
                MetadataTable.Property
            );
            m_codedIndexSizes[(int)CodedIndex.HasCustomAttribute] = ComputeCodedIndexSize(
                5,
                MetadataTable.MethodDef,
                MetadataTable.Field,
                MetadataTable.TypeRef,
                MetadataTable.TypeDef,
                MetadataTable.Param,
                MetadataTable.InterfaceImpl,
                MetadataTable.MemberRef,
                MetadataTable.Module,
                MetadataTable.DeclSecurity,
                MetadataTable.Property,
                MetadataTable.Event,
                MetadataTable.StandAloneSig,
                MetadataTable.ModuleRef,
                MetadataTable.TypeSpec,
                MetadataTable.Assembly,
                MetadataTable.AssemblyRef,
                MetadataTable.File,
                MetadataTable.ExportedType,
                MetadataTable.ManifestResource,
                MetadataTable.GenericParam,
                MetadataTable.GenericParamConstraint,
                MetadataTable.MethodSpec
            );
            m_codedIndexSizes[(int) CodedIndex.HasFieldMarshal] = ComputeCodedIndexSize(
                1,
                MetadataTable.Field,
                MetadataTable.Param
            );
            m_codedIndexSizes[(int) CodedIndex.HasDeclSecurity] = ComputeCodedIndexSize(
                2,
                MetadataTable.TypeDef,
                MetadataTable.MethodDef,
                MetadataTable.Assembly
            );
            m_codedIndexSizes[(int) CodedIndex.MemberRefParent] = ComputeCodedIndexSize(
                3,
                MetadataTable.TypeDef,
                MetadataTable.TypeRef,
                MetadataTable.ModuleRef,
                MetadataTable.MethodDef,
                MetadataTable.TypeSpec
            );
            m_codedIndexSizes[(int) CodedIndex.HasSemantics] = ComputeCodedIndexSize(
                1,
                MetadataTable.Event,
                MetadataTable.Property
            );
            m_codedIndexSizes[(int) CodedIndex.MethodDefOrRef] = ComputeCodedIndexSize(
                1,
                MetadataTable.MethodDef,
                MetadataTable.MemberRef
            );
            m_codedIndexSizes[(int)CodedIndex.MemberForwarded] = ComputeCodedIndexSize(
                1,
                MetadataTable.Field,
                MetadataTable.MethodDef
            );
            m_codedIndexSizes[(int)CodedIndex.Implementation] = ComputeCodedIndexSize(
                2,
                MetadataTable.File,
                MetadataTable.AssemblyRef,
                MetadataTable.ExportedType
            );
            m_codedIndexSizes[(int)CodedIndex.CustomAttributeConstructor] = ComputeCodedIndexSize(
                3,
                MetadataTable.MemberRef
            );
            m_codedIndexSizes[(int)CodedIndex.ResolutionScope] = ComputeCodedIndexSize(
                2,
                MetadataTable.Module,
                MetadataTable.ModuleRef,
                MetadataTable.AssemblyRef,
                MetadataTable.TypeRef
            );
            m_codedIndexSizes[(int)CodedIndex.TypeOrMethodDef] = ComputeCodedIndexSize(
                2,
                MetadataTable.TypeDef,
                MetadataTable.MethodDef
            );
        }

        uint ComputeCodedIndexSize(int bitsToEncode, params MetadataTable[] tables)
        {
            if (tables.Select(GetRowCount).Max() > (1 << (17 - bitsToEncode)) - 1) {
                return 4;
            }
            return 2;
        }

        private PEHeader * PEHeader {
            get {
                if (m_peHeader == null) {
                    m_peHeader = (PEHeader*)(PESignature + 1);
                }
                return m_peHeader;
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
                if (virtualAddress < mid->VirtualAddress) {
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
            var runtimeHeader = m_optionalHeader.CLRRuntimeHeader;
            m_clrHeader = (CLRHeader *)Resolve(runtimeHeader);
            return m_clrHeader != null && m_clrHeader->Verify(m_optionalHeader);
        }

        bool VerifyMetadataRoot()
        {
            //Verify that the contents of the meta-data root, and each stream header are valid. Also, indexes the
            //stream headers.
            m_metadataRoot = (MetadataRoot *)Resolve(m_clrHeader->Metadata);
            if (m_metadataRoot == null) {
                return false;
            }
            return m_metadataRoot->Verify(m_clrHeader->Metadata.Size, out m_streams);
        }

        private void * Resolve(RVAAndSize rva)
        {
            //1. Can we find the section containing the address
            FluentAsserts.Assume(rva.IsConsistent());
            if (rva.IsZero()) {
                return null;
            }
            var pSection = FindSection(rva.RVA);
            
            if (pSection == null)
            {
                return null;
            }

            //2. Does the section have initialized data?
            if (pSection->PointerToRawData == 0 || pSection->SizeOfRawData == 0)
            {
                return null;
            }

            //3. Is the item located entirely within the initialized data portion of the section?
            //Although it is possible that some portion could be placed inside unitialized data,
            //it is unlikely. Such a case is more likely indicative of a malformed image.
            try
            {
                if (checked(rva.RVA - pSection->VirtualAddress) > pSection->SizeOfRawData)
                {
                    return null;
                }

                if (checked(rva.RVA - pSection->VirtualAddress + rva.Size)> pSection->SizeOfRawData) {
                    return null;
                }
            }
            catch (OverflowException)
            {
                return null;
            }
            return ((m_pData + (rva.RVA - pSection->VirtualAddress)) + pSection->PointerToRawData);
        }

        //# Read a string at the specified offset from the "#Strings" heap. Results are cached, so subsequent reads of
        //# the same offset will return the same string instance.
        public String ReadSystemString(uint offset)
        {
            CheckDisposed();
            if (StringStream == null || offset > StringStream->Size) {
                throw new ArgumentOutOfRangeException("offset", "Invalid user string offset.");
            }
            var pString = ((byte*) m_metadataRoot + StringStream->Offset) + offset;
            var length = NativePlatform.Default.StrLen(pString, checked((int) StringStream->Size - (int) offset));
            return new string((sbyte*) pString, 0, length);
        }

        StreamHeader * StringStream
        {
            get
            {
                //If there is no #String stream, then we should not try to read from it.
                if (m_streams == null || m_streams.Length <= (int)StreamID.Strings) {
                    return null;
                }

                return m_streams[(int) StreamID.Strings];
            }
        }

        StreamHeader* BlobStream
        {
            get
            {
                //If there is no #String stream, then we should not try to read from it.
                if (m_streams == null || m_streams.Length <= (int)StreamID.Blob)
                {
                    return null;
                }

                return m_streams[(int)StreamID.Blob];
            }
        }

        public bool IsDisposed
        {
            get { return m_pData == null; }
        }

        public Module Module
        {
            get 
            {
                CheckDisposed();
                if (m_module == null) {
                    if (GetRowCount(MetadataTable.Module) == 0 || m_tables == null || m_tables[(int)MetadataTable.Module] == null) {
                        throw new InvalidOperationException("Missing module table.");
                    }
                    var m = Module.CreateMetadataModule(m_assembly, (ModuleRow *)m_tables[(int)MetadataTable.Module], this);
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_module, m, null);
                    #pragma warning restore 420
                }
                return m_module;
            }
        }

        //Reference: ECMA-335, 5th Edition, Partition II, § 22
        public uint GetRowSize(MetadataTable table)
        {
            CheckDisposed();
            switch (table) {
                case MetadataTable.Assembly:
                    return 16 + StreamID.Blob.IndexSize(this) + 2*StreamID.Strings.IndexSize(this);
                case MetadataTable.AssemblyOS:
                    return 12;
                case MetadataTable.AssemblyProcessor:
                    return 4;
                case MetadataTable.AssemblyRef:
                    return 12+ 2*StreamID.Blob.IndexSize(this) + 2*StreamID.Strings.IndexSize(this);
                case MetadataTable.AssemblyRefOS:
                    return 12 + MetadataTable.AssemblyRef.IndexSize(this);
                case MetadataTable.AssemblyRefProcessor:
                    return 4 + MetadataTable.AssemblyRef.IndexSize(this);
                case MetadataTable.ClassLayout:
                    return 6 + MetadataTable.TypeDef.IndexSize(this);
                case MetadataTable.Constant:
                    return 2 + CodedIndex.HasConstant.IndexSize(this) + StreamID.Blob.IndexSize(this);
                case MetadataTable.CustomAttribute:
                    return
                        CodedIndex.HasCustomAttribute.IndexSize(this)
                        + CodedIndex.CustomAttributeConstructor.IndexSize(this)
                        + StreamID.Blob.IndexSize(this);
                case MetadataTable.DeclSecurity:
                    return 2 + CodedIndex.HasDeclSecurity.IndexSize(this) + StreamID.Blob.IndexSize(this);
                case MetadataTable.EventMap:
                    return MetadataTable.TypeDef.IndexSize(this) + MetadataTable.Event.IndexSize(this);
                case MetadataTable.Event:
                    return 2 + StreamID.Strings.IndexSize(this) + CodedIndex.TypeDefOrRef.IndexSize(this);
                case MetadataTable.ExportedType:
                    return 8 + 2*StreamID.Strings.IndexSize(this) +CodedIndex.Implementation.IndexSize(this);
                case MetadataTable.Field:
                    return 2 + StreamID.Strings.IndexSize(this) + StreamID.Blob.IndexSize(this);
                case MetadataTable.FieldLayout:
                    return 4 + MetadataTable.Field.IndexSize(this);
                case MetadataTable.FieldMarshal:
                    return CodedIndex.HasFieldMarshal.IndexSize(this) + StreamID.Blob.IndexSize(this);
                case MetadataTable.FieldRVA:
                    return 4 + MetadataTable.Field.IndexSize(this);
                case MetadataTable.File:
                    return 4 + StreamID.Strings.IndexSize(this) + StreamID.Blob.IndexSize(this);
                case MetadataTable.GenericParam:
                    return 4 + CodedIndex.TypeOrMethodDef.IndexSize(this) + StreamID.Strings.IndexSize(this);
                case MetadataTable.GenericParamConstraint:
                    return MetadataTable.GenericParam.IndexSize(this) + CodedIndex.TypeDefOrRef.IndexSize(this);
                case MetadataTable.ImplMap:
                    return
                        2
                        + CodedIndex.MemberForwarded.IndexSize(this)
                        + StreamID.Strings.IndexSize(this)
                        + MetadataTable.ModuleRef.IndexSize(this);
                case MetadataTable.InterfaceImpl:
                    return MetadataTable.TypeDef.IndexSize(this) + CodedIndex.TypeDefOrRef.IndexSize(this);
                case MetadataTable.ManifestResource:
                    return 4 + 4 + StreamID.Strings.IndexSize(this) + CodedIndex.Implementation.IndexSize(this);
                case MetadataTable.MemberRef:
                    return 
                        CodedIndex.MemberRefParent.IndexSize(this)
                        + StreamID.Strings.IndexSize(this)
                        + StreamID.Blob.IndexSize(this);
                case MetadataTable.MethodDef:
                    return 
                        8
                        + StreamID.Strings.IndexSize(this)
                        + StreamID.Blob.IndexSize(this)
                        + MetadataTable.Param.IndexSize(this);
                case MetadataTable.MethodImpl:
                    return MetadataTable.TypeDef.IndexSize(this) + 2*CodedIndex.MethodDefOrRef.IndexSize(this);
                case MetadataTable.MethodSemantics:
                    return 2 + MetadataTable.MethodDef.IndexSize(this) + CodedIndex.HasSemantics.IndexSize(this);
                case MetadataTable.MethodSpec:
                    return CodedIndex.MethodDefOrRef.IndexSize(this) + StreamID.Blob.IndexSize(this);
                case MetadataTable.Module:
                    return 2 + StreamID.Strings.IndexSize(this) + 3*StreamID.Guid.IndexSize(this);
                case MetadataTable.ModuleRef:
                    return StreamID.Strings.IndexSize(this);
                case MetadataTable.NestedClass:
                    return 2*MetadataTable.TypeDef.IndexSize(this);
                case MetadataTable.Param:
                    return 4 + StreamID.Strings.IndexSize(this);
                case MetadataTable.Property:
                    return 2 + StreamID.Strings.IndexSize(this) + StreamID.Blob.IndexSize(this);
                case MetadataTable.PropertyMap:
                    return MetadataTable.TypeDef.IndexSize(this) + MetadataTable.Property.IndexSize(this);
                case MetadataTable.StandAloneSig:
                    return StreamID.Blob.IndexSize(this);
                case MetadataTable.TypeDef:
                    return
                        4
                        + 2*StreamID.Strings.IndexSize(this) 
                        + CodedIndex.TypeDefOrRef.IndexSize(this)
                        + MetadataTable.Field.IndexSize(this)
                        + MetadataTable.MethodDef.IndexSize(this);
                case MetadataTable.TypeRef:
                    return CodedIndex.ResolutionScope.IndexSize(this) + 2*StreamID.Strings.IndexSize(this);
                case MetadataTable.TypeSpec:
                    return StreamID.Blob.IndexSize(this);
                default:
                    throw new ArgumentOutOfRangeException("table");
            }
        }

        public uint GetCodedIndexSize(CodedIndex index)
        {
            CheckDisposed();
            if (m_codedIndexSizes == null) {
                throw new InvalidOperationException("Haven't computed coded index sizes yet");
            }
            if (index < 0 || index >= CodedIndex.NUMBER_OF_CODED_INDEX_TYPES) {
                throw new ArgumentOutOfRangeException("index");
            }
            if (m_codedIndexSizes[(int)index] == 0) {
                throw new InvalidOperationException("Missing coded index size");
            }
            return m_codedIndexSizes[(int) index];
        }

        public uint GetTableIndexSize(MetadataTable table)
        {
            CheckDisposed();
            if (GetRowCount(table) >= (1 << 17)) {
                return 4;
            }
            return 2;
        }

        public int GetRowCount(MetadataTable table)
        {
            CheckDisposed();
            FluentAsserts.AssumeNotNull((void *)m_metadataTableHeader);
            return m_metadataTableHeader->GetRowCount(table);
        }

        public string FullPath
        {
            get
            {
                CheckDisposed();
                return m_fullPath;
            }
        }

        private void CheckDisposed()
        {
            if (m_pData == null) {
                throw new ObjectDisposedException("PEFile");
            }
        }

        public uint GetHeapIndexSize(StreamID streamID)
        {
            CheckDisposed();
            if (m_metadataTableHeader == null) {
                throw new InvalidOperationException("Missing metadata table header.");
            }
            return m_metadataTableHeader->GetHeapIndexSize(streamID);
        }

        public void * GetRow(ZeroBasedIndex index, MetadataTable table)
        {
            CheckDisposed();
            table.CheckDefined("table");
            index.CheckGTE(0, "index");
            index.CheckLT(GetRowCount(table), "index");

            if (m_tables == null || m_tables[(int)table] == null) {
                throw new InvalidOperationException("Missing meta-data table.");
            }
            return checked(m_tables[(int)table] + index.Value*GetRowSize(table));
        }
        
        public IReadOnlyList<byte> ReadBlob(uint offset)
        {
            CheckDisposed();
            CheckDisposed();
            if (StringStream == null || offset > BlobStream->Size) {
                throw new ArgumentOutOfRangeException("offset", "Invalid blog offset.");
            }
            var pBlob = ((byte*)m_metadataRoot + BlobStream->Offset) + offset;
            var b0 = *pBlob;
            uint length;
            
            if ((b0 & 0x80) == 0x00) {
                length = b0;
                ++pBlob;
            }
            else if ((b0 & 0xC0) == 0x80) {
                length = ((b0 & ~0x80U) << 8) | *(pBlob + 1);
                pBlob += 2;
            }
            else {
                length = 
                    ((b0 & ~0xC0U) << 24)
                    | (((uint)*(pBlob + 1)) << 16)
                    | (((uint)*(pBlob + 2)) << 8)
                    | ((uint)*(pBlob + 3));
                pBlob += 4;
            }
            return new BufferWrapper(pBlob, checked((int)length));
        }

        public Guid ReadGuid(uint offset)
        {
            CheckDisposed();
            //TODO: Implement this
            throw new NotImplementedException();
        }

        public bool IsSorted(MetadataTable table)
        {
            CheckDisposed();
            return (m_metadataTableHeader->ValidTables & (1UL << (int) table)) != 0;
        }

        public ZeroBasedIndex GetRowIndex(MetadataTable table, void* pRow)
        {
            CheckDisposed();
            table.CheckDefined("table");
            FluentAsserts.CheckNotNull((void *)pRow, "pRow");

            var pFirst = GetRow(0.ToZB(), table);
            if (
                pRow < pFirst 
                || (((byte*)pRow - (byte*)pFirst) % GetRowSize(table)) != 0
            ) {
                throw new ArgumentException(string.Format("Not a valid row in {0}", table), "pRow");
            }

            var index = ((byte*) pRow - (byte*) pFirst)/GetRowSize(table);
            if (index >= GetRowCount(table)) {
                throw new ArgumentException(string.Format("Not a valid row in {0}", table), "pRow");
            }

            return ((int)index).ToZB();
        }

        //# Returns the index of the largest item in [table] strictly less than (<) [value], or -1 if no such value exists.
        //#
        //# Parameters
        //# ============
        //# [table] : The metadata table to search.
        //# [value] : The value to search for.
        //# [selector]
        //#     A function that projects the desired sort field(s) from the rows of the table. The table must be sorted\
        //#     by selector.
        public ZeroBasedIndex GreatestLowerBound<T>(MetadataTable table, T value, UnsafeSelector<T> selector)
        {
            CheckDisposed();
            table.CheckDefined("table");
            selector.CheckNotNull("selector");

            if (! IsSorted(table)) {
                throw new InvalidOperationException(string.Format("The table '{0}' is not sorted", table));
            }

            if (GetRowCount(table) == 0) {
                return new ZeroBasedIndex(-1);
            }

            var min = 0.ToZB();
            var max = GetRowCount(table).ToZB() - 1;
            var comparer = Comparer<T>.Default;

            while (max > 0 && max != min) {
                var mid = ((max - min) + 1)/2 + min;
                var comp = comparer.Compare(value, selector(GetRow(mid, table)));
                if (comp <= 0) {
                    max = mid - 1;
                }
                else {
                    min = mid;
                }
            }

            if (max < 0) {
                return (-1).ToZB();
            }

            if (min > 0 || comparer.Compare(value, selector(GetRow(min, table))) > 0) {
                return min;
            }
            return (-1).ToZB();
        }

        //# Returns the index of the smallest item in [table] strictly greater than (>) [value]. If no such element
        //# exists then the index immedietly beyond the end of the table (count + 1) is returned.
        //#
        //# Parameters
        //# ============
        //# [table] : The metadata table to search.
        //# [value] : The value to search for.
        //# [selector]
        //#     A function that projects the desired sort field(s) from the rows of the table. The table must be sorted\
        //#     by selector.
        public ZeroBasedIndex LeastUpperBound<T>(MetadataTable table, T value, UnsafeSelector<T> selector)
        {
            CheckDisposed();
            table.CheckDefined("table");
            selector.CheckNotNull("selector");

            if (!IsSorted(table)) {
                throw new InvalidOperationException(string.Format("The table '{0}' is not sorted", table));
            }

            if (GetRowCount(table) == 0) {
                return 1.ToZB();
            }

            var min = 0.ToZB();
            var max = GetRowCount(table).ToZB() - 1;
            var last = max;
            var comparer = Comparer<T>.Default;

            while (min < last && max != min) {
                var mid = (max - min)/2 + min;
                var comp = comparer.Compare(value, selector(GetRow(mid, table)));
                if (comp < 0) {
                    max = mid;
                }
                else {
                    min = mid + 1;
                }

            }

            if (min > last) {
                return last + 1;
            }
            if (max < last || comparer.Compare(value, selector(GetRow(max, table))) > 0) {
                return max;
            }
            return last + 1;
        }

        public ZeroBasedIndex Find<T>(MetadataTable table, T value, UnsafeSelector<T> selector)
        {
            CheckDisposed();
            //TODO Implement this
            throw new NotImplementedException();
        }

        public Type ParseTypeSpec(ZeroBasedIndex index, Module module)
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

        public Type ResolveTypeRef(ZeroBasedIndex index, Module module)
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

        public Type ParseFieldSignature(uint offset, TypeDefinition declaringType)
        {
            CheckDisposed();
            var blob = ReadBlob(offset);
            return SignatureParser.ParseFieldSignature(blob, declaringType);
        }

        public LiftedList<TChild> LoadIndirectChildren<TChild, TParentField>(
            TParentField parent,
            MetadataTable childTable,
            UnsafeSelector<TParentField> parentSelector,
            CreateObjectDelegate<TChild> factory
        ) where TChild : class
        {
            CheckDisposed();
            //It is valid for the child table to not be sorted, but I don't expect such a case to occur in practice.
            //I imagine that this could maybe happen with ENC, but we don't need to be able to decompile enc assemblies.
            //In either case, if we do end up needing to support assemblies with unsorted meta-data tables, then we should probably
            //add our best attempt at an efficent fallback in that case. For now we just throw.
            IsSorted(childTable).Assume("The generic param constraint table is not sorted.");

            var glb = GreatestLowerBound(
                childTable,
                parent,
                parentSelector
            );
            var lub = LeastUpperBound(
                childTable,
                parent,
                parentSelector
            );

            var ret = new LiftedList<TChild>(
                (glb - lub - 1).Value,
                index => GetRow(index.ToZB() + lub + 1, childTable),
                factory,
                () => IsDisposed
            );

            return ret;
        }

        public LiftedList<CustomAttribute> LoadCustomAttributes(MetadataTable parentTable, void * parentRow)
        {
            return Module.PEFile.LoadIndirectChildren(
                new HasCustomAttribute(parentTable, GetRowIndex(parentTable, parentRow)),
                MetadataTable.CustomAttribute,
                pRow => ((CustomAttributeRow*)pRow)->GetParent(this),
                pRow => new CustomAttribute((CustomAttributeRow*)pRow, this)
            );
        }

        private IntPtr GetRowSafe(ZeroBasedIndex index, MetadataTable table)
        {
            return (IntPtr) GetRow(index, table);
        }

        public IEnumerable<IntPtr> GenericParameterRows()
        {
            for (int i = 0; i < GetRowCount(MetadataTable.GenericParam); ++i) {
                yield return GetRowSafe(i.ToZB(), MetadataTable.GenericParam);
            }
        }

        public LiftedList<T> LoadDirectChildren<T>(
            MetadataTable childTable,
            GetTokenDelegate tokenSelector,
            CreateObjectDelegate<T> factory,
            MetadataTable parentTable,
            void* parentRow
        ) where T : class
        {
            factory.CheckNotNull("factory");
            return LoadDirectChildren(childTable, tokenSelector, (pRow, index)=>factory(pRow), parentTable, parentRow);
        }

        public LiftedList<T> LoadDirectChildren<T>(
            MetadataTable childTable,
            GetTokenDelegate tokenSelector,
            CreateObjectDelegateEX<T> factory,
            MetadataTable parentTable,
            void* parentRow
        ) where T : class
        {
            var firstMemberIndex = (ZeroBasedIndex)tokenSelector(parentRow);
            ZeroBasedIndex lastMemberIndex;
            var tableIndex = GetRowIndex(parentTable, parentRow);
            if (tableIndex == GetRowCount(parentTable) - 1) {
                lastMemberIndex = new ZeroBasedIndex(GetRowCount(childTable));
            }
            else {
                lastMemberIndex = (ZeroBasedIndex)tokenSelector(GetRow(tableIndex + 1, parentTable));
            }
            var ret = new LiftedList<T>(
                (lastMemberIndex - firstMemberIndex).Value,
                index => GetRow(firstMemberIndex + index, childTable),
                factory,
                () => IsDisposed
            );
            return ret;
        }


        public void Dispose()
        {
            m_assembly = null;
            m_metadataTableHeader = null;
            m_pData = null;
            m_fileSize = 0;
            m_peHeader = null;
            m_optionalHeader = null;
            m_clrHeader = null;
            m_streams = null;
            m_fullPath = null;

            if (m_tables != null) {
                for (var i = 0; i < m_tables.Length; ++i) {
                    m_tables[i] = null;
                }
                m_tables = null;
            }

            if (m_memoryMap != null) {
                m_memoryMap.Dispose();
                m_memoryMap = null;
            }
        }
    }
}

