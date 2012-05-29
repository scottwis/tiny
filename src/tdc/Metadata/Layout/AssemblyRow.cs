// AssemblyRow.cs
//  
// Author:
//     Scott Wisniewski <scott@scottdw2.com>
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

using System.Configuration.Assemblies;
using System.Runtime.InteropServices;

namespace Tiny.Decompiler.Metadata.Layout
{
    //# Defines the memory layout for a row in the Assembly table in a managed executable.
    //# reference: ECMA-335, 5th Edition, Partition II, § 22.2
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct AssemblyRow
    {
        //# Specifies the algorithm that should be used to compute the assembly hash (which is present in the
        //# AssemblyRef table), I think. There's not a lot of info about it in the spec.
        public AssemblyHashAlgorithm HashAlgorithm
        {
            get
            {
                fixed (AssemblyRow* pThis = &this) {
                    return (AssemblyHashAlgorithm) (*((uint*) pThis));
                }
            }
        }

        //# The major version # of the assembly.
        [FieldOffset(4)] public readonly ushort MajorVersion;
        //# The minor version # of the assembly.
        [FieldOffset(6)] public readonly ushort MinorVersion;
        //# The "build number" component of the assembly version (*.*.BuildNumber).
        [FieldOffset(8)] public readonly ushort BuildNumber;
        //# The "revision number" component of the assembly version (*.*.*.RevisionNumber).
        [FieldOffset(10)] public readonly ushort RevisionNumber;
        //# A bit mask of flags related to the assembly. The tiny decompiler generally ignores this field.
        [FieldOffset(12)] public readonly AssemblyFlags Flags;

        //# Returns the index of the public key in the blob heap. 
        public uint GetPublicKeyIndex(PEFile peFile)
        {
            peFile.AssumeNotNull();
            fixed (AssemblyRow* pThis = &this) {
                var pPublicKey = (byte*) pThis + 16;

                if (peFile.GetHeapIndexSize(StreamID.Blob) == 2) {
                    return *(ushort*) pPublicKey;
                }
                return *(uint*) pPublicKey;
            }
        }

        public uint GetNameOffset(PEFile peFile)
        {
            peFile.AssumeNotNull();
            fixed (AssemblyRow* pThis = &this) {
                var pName = (byte*) pThis + 16 + peFile.GetHeapIndexSize(StreamID.Blob);
                if (peFile.GetHeapIndexSize(StreamID.Strings) == 2) {
                    return *(ushort*) pName;
                }
                return *(uint*) pName;
            }
        }

        public uint GetCultureOffset(PEFile peFile)
        {
            peFile.AssumeNotNull();
            fixed (AssemblyRow* pThis = &this) {
                var pCulture = (byte*) pThis + 16 + peFile.GetHeapIndexSize(StreamID.Blob) + peFile.GetHeapIndexSize(StreamID.Strings);
                if (peFile.GetHeapIndexSize(StreamID.Strings) == 2) {
                    return *(ushort*) pCulture;
                }
                return *(uint*) pCulture;
            }
        }
    }
}
