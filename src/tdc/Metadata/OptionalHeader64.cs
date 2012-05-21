// 
// OptionalHeader64.cs
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
    //# An implementation of [OptionalHeader] for [FileFormat.PE32_PLUS][PE32_PLUS] format files.
    sealed unsafe class OptionalHeader64 : OptionalHeader
    {
        private OptionalHeaderLayout64 * m_pLayout;

        public OptionalHeader64(OptionalHeaderLayout64 * pLayout)
        {
            m_pLayout = pLayout;
        }

        public override FileFormat MagicNumber
        {
            get { return m_pLayout->MagicNumber; }
        }
        public override byte MajorLinkerVersion
        {
            get { return m_pLayout->MajorLinkerVersion; }
        }
        public override byte MinorLinkerVersion
        {
            get { return m_pLayout->MinorLinkerVersion; }
        }
        public override uint CodeSize
        {
            get { return m_pLayout->CodeSize; }
        }
        public override  uint InitializedDataSize
        {
            get { return m_pLayout->InitializedDataSize; }
        }
        public override uint UninitializedDataSize
        {
            get { return m_pLayout->UninitializedDataSize; }
        }
        public override uint AddressOfEntryPoint
        {
            get { return m_pLayout->AddressOfEntryPoint; }
        }
        public override uint BaseOfCode
        {
            get { return m_pLayout->BaseOfCode; }
        }
        public override ulong ImageBase
        {
            get { return m_pLayout->ImageBase; }
        }
        public override uint SectionAlignment
        {
            get { return m_pLayout->SectionAlignment; }
        }
        public override uint FileAlignment
        {
            get { return m_pLayout->FileAlignment; }
        }
        public override  ushort MajorOSVersion
        {
            get { return m_pLayout->MajorOSVersion; }
        }
        public override ushort MinorOSVersion
        {
            get { return m_pLayout->MinorOSVersion; }
        }
        public override ushort MajorImageVersion
        {
            get { return m_pLayout->MajorImageVersion; }
        }
        public override ushort MajorSubsystemVersion
        {
            get { return m_pLayout->MajorSubsystemVersion; }
        }
        public override ushort MinorSubsystemVersion
        {
            get { return m_pLayout->MinorSubsystemVersion; }
        }
        public override uint Win32VersionValue
        {
            get { return m_pLayout->Win32VersionValue; }
        }
        public override uint ImageSize
        {
            get { return m_pLayout->ImageSize; }
        }
        public override uint HeaderSize
        {
            get { return m_pLayout->HeaderSize; }
        }
        public override uint Checksum
        {
            get { return m_pLayout->Checksum; }
        }
        public override ImageSubsystem Subsystem
        {
            get { return m_pLayout->Subsystem; }
        }
        public override DllCharacteristics DllCharacteristics
        {
            get { return m_pLayout->DllCharacteristics; }
        }
        public override ulong StackReserveSize
        {
            get { return m_pLayout->StackReserveSize; }
        }
        public override ulong StackCommitSize
        {
            get { return m_pLayout->StackCommitSize; }
        }
        public override ulong HeapReserveSize
        {
            get { return m_pLayout->HeapReserveSize; }
        }
        public override ulong HeapCommitSize
        {
            get { return m_pLayout->HeapCommitSize; }
        }
        public override uint LoaderFlags
        {
            get { return m_pLayout->LoaderFlags; }
        }
        public override uint NumberOfDataDirectories
        {
            get { return m_pLayout->NumberOfDataDirectories; }
        }
        public override  RVAAndSize * DataDirectories
        {
            get { return (RVAAndSize *)(m_pLayout + 1); }
        }
        protected override uint LAYOUT_SIZE {
            get { return (uint)sizeof(OptionalHeaderLayout64); }
        }
        protected override ulong SIZE_MAX {
            get { return ulong.MaxValue; }
        }

        public override byte* GetAddress()
        {
            return (byte *)m_pLayout;
        }
    }
}

