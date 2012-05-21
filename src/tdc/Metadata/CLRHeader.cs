// CLRHeader.cs
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

using System.Runtime.InteropServices;

namespace Tiny.Decompiler.Metadata
{

    //# Defines the layout of the CLR header in a managed executable.
    //# Reference: Ecma-335 Spec, 5th Edtion, Partition II § 25.3.3
    [StructLayout(LayoutKind.Explicit)]
    struct CLRHeader
    {
        //# The size of the header, in bytes.
        [FieldOffset(0)]
        public readonly uint HeaderSize;

        //# The minimum major version of the runtime required to run this program. The tiny decompiler will only
        //# accept images with a clr version between 2.0 and 2.5, inclusive.
        [FieldOffset(4)]
        public readonly ushort MajorRuntimeVersion;

        //# The minimum minor version of the runtime required to run this program.
        [FieldOffset(6)]
        public readonly ushort MinorRuntimeVersion;

        //# The RVA / Size of the physical metadata. 
        [FieldOffset(8)] 
        public readonly RVAAndSize Metadata;

        //# CLR specific flags describing the image.
        [FieldOffset(16)]
        public readonly CLRHeaderFlags Flags;

        //# The token for the entry point for the image. This will be a MethodDef token if the entry point is located
        //# in the current module, and a File token if the assembly defines multiple modules and the entry point is
        //# not located in the manifest module.
        [FieldOffset(20)] 
        public readonly MetadataToken EntryPointToken;

        //# Beats me. The Ecma 335 Spec (5th edition Partion II, § 25.3.3) simply says that this contains the RVA/Size
        //# of "implementation-sepcific resources".  We haven't investigated what that might mean. The tiny
        //# decompiler will conservatively reject any image with a non-zero value.
        [FieldOffset(24)] 
        public readonly RVAAndSize Resources;

        //# RVA / size of the hash data for this PE file. It is used by the CLR loader for binding and versioning.
        [FieldOffset(32)]
        public readonly RVAAndSize StrongNameSignature;

        //# I think this has something to do with ENC (edit and continue). Partion II, § 25.3.3 of the ECMA spec
        //# simply says that the value should be "set to 0 on write and ignored on read".
        [FieldOffset(40)]
        public readonly RVAAndSize CodeManagerTable;

        //# The RVA / size of an array of locations in the file that contain an array of function pointers. This is
        //# likely only used by managed C++. Any image with a non-zero value will be rejected by tdc.
        [FieldOffset(48)]
        public readonly RVAAndSize VTableFixups;

        //# The ECMA spec simply says that this should be written as 0, and should be ignored on read. It's not clear what
        //# it is used for.
        [FieldOffset(56)]
        public readonly RVAAndSize ExportAddressTableJumps;

        //# The ECMA spec simply says that this should be written as 0, and should be ignored on read. It's not clear what
        //# it is used for.
        [FieldOffset(64)]
        public readonly RVAAndSize ManagedNativeHeader;

        public unsafe bool Verify(OptionalHeader optionalHeader)
        {
            //1. The header size must be consistent with the other places it is reported.
            if (HeaderSize < sizeof(CLRHeader)) {
                return false;
            }

            if (HeaderSize < optionalHeader.CLRRuntimeHeader->Size) {
                return false;
            }

            //2. We require metadata versions between 2.0 and 2.5.
            if (MajorRuntimeVersion != 2) {
                return false;
            }

            if (MinorRuntimeVersion > 5) {
                return false;
            }
            //3. The total size of the meta-data must be at least large enough to store the metadata root.
            if (Metadata.RVA == 0 || Metadata.Size < sizeof(MetadataRoot)) {
                return false;
            }

            //4. If any "uknown" flags are present, we reject the image.
            if ((Flags & ~CLRHeaderFlags.KnownFlags) != 0) {
                return false;
            }

            //4. We don't support mixed-mode assemblies.
            const CLRHeaderFlags requiredFlags = CLRHeaderFlags.COMIMAGE_FLAGS_ILONLY;
            if ((Flags & requiredFlags) != requiredFlags) {
                return false;
            }

            const CLRHeaderFlags disallowedFlags = CLRHeaderFlags.COMIMAGE_FLAGS_NATIVE_ENTRYPOINT;
            if ((Flags & disallowedFlags) != 0) {
                return false;
            }

            //5. The StrongNameSignature pointer should have a zero size only if it is null.
            if (! StrongNameSignature.IsConsistent()) {
                return false;
            }

            //6. We should have a Strong name pointer iif the image is marked as being strong named signed.
            if (StrongNameSignature.IsZero() != ((Flags & CLRHeaderFlags.COMIMAGE_FLAGS_STRONGNAMESIGNED) == 0)) {
                return false;
            }

            //7. There should be a null code manager table pointer.
            if (! CodeManagerTable.IsZero()) {
                return false;
            }

            //8. There should be no vtable fixups.
            if (!VTableFixups.IsZero())
            {
                return false;
            }

            return true;
        }
    }
}
