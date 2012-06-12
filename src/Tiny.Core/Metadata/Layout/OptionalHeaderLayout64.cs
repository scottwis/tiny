// 
// OptionalHeaderLayout64.cs
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

using System.Runtime.InteropServices;

namespace Tiny.Metadata.Layout
{
    //# Defines the memory layout for an optional pe header in [FileFormat.PE32_PLUS][PE32_PLUS]
    //# format files.
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct OptionalHeaderLayout64
    {
        [FieldOffset(0)] public readonly FileFormat MagicNumber;
        [FieldOffset(2)] public readonly byte MajorLinkerVersion;
        [FieldOffset(3)] public readonly byte MinorLinkerVersion;
        [FieldOffset(4)] public readonly uint CodeSize;
        [FieldOffset(8)] public readonly uint InitializedDataSize;
        [FieldOffset(12)] public readonly uint UninitializedDataSize;
        [FieldOffset(16)] public readonly uint AddressOfEntryPoint;
        [FieldOffset(20)] public readonly uint BaseOfCode;
        [FieldOffset(24)] public readonly ulong ImageBase;
        [FieldOffset(32)] public readonly uint SectionAlignment;
        [FieldOffset(36)] public readonly uint FileAlignment;
        [FieldOffset(40)] public readonly ushort MajorOSVersion;
        [FieldOffset(42)] public readonly ushort MinorOSVersion;
        [FieldOffset(44)] public readonly ushort MajorImageVersion;
        [FieldOffset(46)] public readonly ushort MinorImageVersion;
        [FieldOffset(48)] public readonly ushort MajorSubsystemVersion;
        [FieldOffset(50)] public readonly ushort MinorSubsystemVersion;
        [FieldOffset(52)] public readonly uint Win32VersionValue;
        [FieldOffset(56)] public readonly uint ImageSize;
        [FieldOffset(60)] public readonly uint HeaderSize;
        [FieldOffset(64)] public readonly uint Checksum;
        [FieldOffset(68)] public readonly ImageSubsystem Subsystem;
        [FieldOffset(70)] public readonly DllCharacteristics DllCharacteristics;
        [FieldOffset(72)] public readonly ulong StackReserveSize;
        [FieldOffset(80)] public readonly ulong StackCommitSize;
        [FieldOffset(88)] public readonly ulong HeapReserveSize;
        [FieldOffset(96)] public readonly ulong HeapCommitSize;
        [FieldOffset(104)] public readonly uint LoaderFlags;
        [FieldOffset(108)] public readonly uint NumberOfDataDirectories;
    }
}

