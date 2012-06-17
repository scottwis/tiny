// 
// OptionalHeader.cs
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
    //# An abstract base class for the optional header in a PE file.
    //# Currently there are 2 implementations:
    //#
    //# 1. [OptionalHeader32] describes files with a [MagicNumber] of [FileFormat.PE32]
    //# 2. [OptionalHeader64] describes files with a [MagicNumber] of [FileFormat.PE32_PLUS]
    abstract unsafe class OptionalHeader
    {
        //# A "magic number" identifying the file format of the PE. Only [PE32] and [PE32_PLUS]
        //# are supported by tdc.
        public abstract FileFormat MagicNumber{ get; }
        public abstract byte MajorLinkerVersion { get; }
        public abstract byte MinorLinkerVersion { get; }
        //# The total size of all code sections in the executable.
        public abstract uint CodeSize { get; }

        //# The total size of all initialized data sections in the executable.
        public abstract uint InitializedDataSize { get; }

        //# The total size of all unintialized data sections in the executable.
        public abstract uint UninitializedDataSize { get; }

        //# The RVA of the entry point of the executable. This is an offset relative to the ACTUAL base address of the
        //# image when it is loaded into memory.
        public abstract uint AddressOfEntryPoint { get; }

        //# The RVA of the start of the code section containing [AddressOfEntryPoint]. This is an offset
        //# relative to the ACTUAL base address of the image when it is loaded into memory.
        public abstract uint BaseOfCode { get; }

        //# The preferred virtual address of the first byte of the image when it is loaded into memory.
        //# For P332 format files this should be a 32 bit result zero-extended to 64 bits.
        public abstract ulong ImageBase { get; }

        //# The alignment (in bytes) of sections when they are loaded into memory. It must be >= [FileAlignment].
        //# The default should be the architecture specific page size.
        public abstract uint SectionAlignment {get; }

        //# The alignment (in bytes) of raw section data within the file. It must be a power of 2 between 512 and 64K.
        //# If [SectionAlignment] is less than the architecture's page size, then `SectionAlignment` must equal
        //# `FileAlignment`. For most managed executables this should be 0x200 (512 bytes),
        //# although tdc will accept any valid value.
        public abstract uint FileAlignment { get; }

        //# The major version number of the minimum required operating system. The ECMA-335 spec says this should be
        //# 5, but that it can be ignored on read. The tiny decompiler ingores this vlaue.
        public abstract ushort MajorOSVersion { get; }

        //# The minor version number of the minimum required operating system. The ECMA-335 specs says this value
        //# should be 0, but that it can be ignored on read. The tiny decompiler ingores this vlaue.
        public abstract ushort MinorOSVersion { get; }

        //# The major version number of the image. The tiny decompiler will accept any arbitrary value for this field.
        public abstract ushort MajorImageVersion { get; }

        //# The major version number of the Windows Subsystem required by this image. The ECMA 335 spec says this
        //# should be 5, but that the value can be ignored on read. The tiny decompiler will conservatively reject
        //# any file with a value < 5.
        public abstract ushort MajorSubsystemVersion { get; }

        //# The minor version number of the Windows Subsystem required by this image. The ECMA 335 spec says this
        //# should be 0, but that the value can be ignored on read. The tiny decompiler will acccept any arbitrary value
        //# for this field.
        public abstract ushort MinorSubsystemVersion { get; }

        //# Reserved. The tiny decompiler will conservatively reject any file with this field not set to 0.
        public abstract uint Win32VersionValue { get; }

        //# The size of the image, including all headers, when it is loaded in memory. It must be a multiple of
        //# [SectionAlignment].
        public abstract uint ImageSize { get; }

        //# The combined size of the MSDOS Heaader, PE Header, PE Optional Header, and any necessary padding bytes.
        //# It must be a multiple of [FileAlignment]
        public abstract uint HeaderSize { get; }

        //# A checksum for the image file. This is ignored by the tiny decompiler.
        public abstract uint Checksum { get; }

        //# The Windows Subsystem required to run the image. Only [IMAGE_SUBSYSTEM_WINDOWS_CUI] and
        //# [IMAGE_SUBSYSTEM_WINDOWS_GUI] are supported by tdc.
        public abstract ImageSubsystem Subsystem {get; }

        //# A bit flag describing various options to the Windows Loader.
        //# None of the bit flags marked as "reserved" are supported by tdc. Any image containing a DllCharacteristics
        //# value with a reserved bit set to 1 will be conservatively rejected by the tiny decompiler.
        //# Finally, images with [IMAGE_DLLCHARACTERISTICS_WDM_DRIVER] set to 1 will also be rejected.
        public abstract DllCharacteristics DllCharacteristics { get; }

        //# The size, in bytes, of virtual memory address space to reserve for the initial thread of the image. Only
        //# [StackCommitSize] bytes are initiallly commited. The reset is commited, one page at a time, until the
        //# reserve size is reached.
        //#
        //# For [FileFormat.PE32][PE32] images this should be a 32 bit value zero-extended to 64 bits.
        //# This field is ignored by tdc.
        public abstract ulong StackReserveSize { get; }

        //# The size, in bytes, of virtual memory address space to commit for the initial thread of the image.
        //# For [FileFormat.PE32][PE32] images this should be a 32 bit value zero-extended to 64 bits.
        public abstract ulong StackCommitSize { get; }

        //# The size, in bytes, of virtual memory address space to reserve for the default Windows Process Heap.
        //# Only [HeapCommitSize] bytes are initially commited. The rest is made available one page at a time
        //# until the reserve size is reached. For [FileFormat.PE32][PE32] images this should be a 32 bit value
        //# zero-extended to 64 bits.
        public abstract ulong HeapReserveSize { get; }

        //# The size, in bytes, of virtual memory address space to commit for the the default Windows Process Heap.
        //# For [FileFormat.PE32][PE32] images this should be a 32 bit value zero-extended to 64 bits.
        public abstract ulong HeapCommitSize {get; }

        //# This field is reserved as must be zero. Both the ECMA 335 and PE/COFF specs identify it as the
        //# "loader flags" field, and simply say that it is reserved and must be 0. The tiny decompiler will
        //# conservatively reject any image that has this field not equal to 0.
        public abstract uint LoaderFlags {get; }

        //# The number of data directories in the optional header.
        //# The tiny decompiler will reject any image with this value not set to 0.
        public abstract uint NumberOfDataDirectories { get; }

        //# Returns a pointer to array of DataDirectories stored in the header.
        //# We verify that the each entry
        public abstract RVAAndSize * DataDirectories { get; }

        public abstract byte* GetAddress();

        protected abstract uint LAYOUT_SIZE { get; }
        protected abstract ulong SIZE_MAX { get; }

        private RVAAndSize * GetDataDirectory(int index)
        {
            if (index >= 0 && index < NumberOfDataDirectories) {
                return DataDirectories + index;
            }
            throw new IndexOutOfRangeException();
        }

        //# A convience property for accessing the CLR Header.
        //# This is equivalent to `DataDirectories[14]`.
        public RVAAndSize CLRRuntimeHeader {
            get { return *GetDataDirectory(14); }
        }

        public bool Verify(uint fileSize, PEHeader * pPEHeader)
        {
            FluentAsserts.AssumeNotNull((void *)pPEHeader);
            fileSize.AssumeGT(0U);

            if (CodeSize > ImageSize) {
                return false;
            }

            if (InitializedDataSize > ImageSize) {
                return false;
            }

            if (UninitializedDataSize > ImageSize) {
                return false;
            }

            try {
                if (checked(CodeSize + InitializedDataSize + UninitializedDataSize) > ImageSize) {
                    return false;
                }
            }
            catch (OverflowException) {
                return false;
            }

            if (AddressOfEntryPoint != 0 && AddressOfEntryPoint < BaseOfCode) {
                return false;
            }

            if (SectionAlignment < FileAlignment) {
                return false;
            }

            if (FileAlignment < 512 || FileAlignment > 0x10000) {
                return false;
            }

            //Is file alignment a power of 2?
            if (((FileAlignment) & (FileAlignment - 1)) != 0) {
                return false;
            }

            if (Win32VersionValue != 0) {
                return false;
            }

            //Is the header size a multiple of the file alignment?
            if ((HeaderSize & (FileAlignment - 1)) != 0) {
                return false;
            }

            if (HeaderSize > fileSize) {
                return false;
            }

            try {
                //Is the reported header large enough to actually hold all the headers?
                if (
                    HeaderSize < checked(
                        LAYOUT_SIZE
                        + (NumberOfDataDirectories * sizeof(RVAAndSize))
                        + sizeof(PEHeader)
                        + PEFile.MSDosStubSize
                        + (pPEHeader->NumberOfSections * sizeof(SectionHeader))
                    )
                ) {
                    return false;
                }
            }
            catch (OverflowException) {
                return false;
            }

            if (
                Subsystem != ImageSubsystem.IMAGE_SUBSYSTEM_WINDOWS_CUI
                && Subsystem != ImageSubsystem.IMAGE_SUBSYSTEM_WINDOWS_GUI
            ) {
                return false;
            }

            if ((DllCharacteristics.RESERVED_BITS & DllCharacteristics) != 0) {
                return false;
            }

            if ((DllCharacteristics & DllCharacteristics.IMAGE_DLLCHARACTERISTICS_WDM_DRIVER) != 0) {
                return false;
            }

            if (StackReserveSize < StackCommitSize) {
                return false;
            }

            StackReserveSize.AssumeLTE(SIZE_MAX);
            StackCommitSize.AssumeLTE(SIZE_MAX);

            if (HeapReserveSize < HeapCommitSize) {
                return false;
            }

            HeapReserveSize.AssumeLTE(SIZE_MAX);
            HeapCommitSize.AssumeLTE(SIZE_MAX);

            if (LoaderFlags != 0) {
                return false;
            }

            if (NumberOfDataDirectories < 16) {
                return false;
            }

            if (! DataDirectories[15].IsZero()) {
                return false;
            }

            if (CLRRuntimeHeader.IsZero()) {
                return false;
            }

            if (CLRRuntimeHeader.Size < sizeof(CLRHeader)) {
                return false;
            }

            for (int i = 0; i < NumberOfDataDirectories; ++i) {
                if (DataDirectories[i].RVA > ImageSize) {
                    return false;
                }

                if (! DataDirectories[i].IsConsistent()) {
                    return false;
                }
            }

            return true;
        }
    }
}

