// MetadataRoot.cs
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

using System;
using System.Runtime.InteropServices;
using Tiny.Decompiler.Interop;

namespace Tiny.Decompiler.Metadata
{
    //# Defines the layout of the "Metadata root" header in a managed PE/COFF file.
    //# Reference: ECMA-335, 5th Edition, Partition II, § 24.2.1
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct MetadataRoot
    {
        public static uint MinSize = 20;

        //# The magic signature for CLR metadata. Must be 0x424A5342
        [FieldOffset(0)]
        public readonly uint Signature;

        //# Ignored.
        [FieldOffset(4)]
        public readonly ushort MajorVersion;

        //# Ignored.
        [FieldOffset(6)]
        public readonly ushort MinorVersion;

        //# Reserved. Must be zero. The tiny decompiler will reject any image where this field is not 0.
        [FieldOffset(8)]
        public readonly uint Reserved;

        //# The length of the version string, rounded up to the next 4 byte boundary.
        [FieldOffset(12)]
        public readonly uint Length;

        public string GetVersion()
        {
            if (Length > 255) {
                throw new InvalidOperationException("The version string has an invalid length and cannot be read.");
            }
            fixed  (MetadataRoot * pThis= &this) {
                return new string(
                    (sbyte *)pThis + 16,
                    0, 
                    NativePlatform.Default.StrLen((byte *)pThis + 16, checked((int)Length))
                );
            }
        }

        //# Reserved. Must be zero. The tiny decompiler will reject any image where this field is not 0.
        public ushort Flags
        {
            get
            {
                if (Length > 255) {
                    throw new InvalidOperationException("Invalid meta-data root.");
                }
                fixed (MetadataRoot * pThis = &this) {
                    return *(ushort*) ((byte *)pThis + Length + 16);
                }
            }
        }

        //# The number of stream headers.
        public ushort NumberOfStreams
        {
            get
            {
                if (Length > 255) {
                    throw new InvalidOperationException("Invalid meta-data root.");
                }
                fixed (MetadataRoot * pThis  = &this) {
                    return *(ushort*) ((byte*) pThis + Length + 18);
                }
            }
        }

        //# Returns a pointer to the FirstStreamHeader. StreamHeaders are variable length, so they must be read
        //# sequentally.
        public StreamHeader * FirstStreamHeader
        {
            get
            {
                fixed (MetadataRoot* pThis = &this)
                {
                    return (StreamHeader*)((byte*)pThis + Length + 20);
                }
            }
        }

        public bool Verify(uint maxSize, out StreamHeader * [] streams)
        {
            streams = new StreamHeader *[(int)StreamID.NUMBER_OF_STREAMS];

            if (Signature != 0x424A5342) {
                return false;
            }

            if (Reserved != 0) {
                return false;
            }

            if (Length > 256 || (Length % 4) != 0) {
                return false;
            }

            try {
                if (checked(Length + MinSize + sizeof(ushort) * 2) > maxSize)
                {
                    return false;
                }
            }
            catch (OverflowException) {
                return false;
            }

            if (!VerifyVersion()) {
                return false;
            }

            if (Flags != 0) {
                return false;
            }

            if (NumberOfStreams < 1 || NumberOfStreams > 5) {
                return false;
            }

            var pStream = FirstStreamHeader;
            
            fixed (MetadataRoot* pThis = &this) {
                for (var i = 0; i < NumberOfStreams; ++i) {
                    if (checked((byte*) pStream - (byte*) pThis > maxSize - StreamHeader.MinSize)) {
                        return false;
                    }
                    uint maxLength;
                    try {
                        maxLength = checked(maxSize - StreamHeader.MinSize - 4u - (uint) ((byte*) pStream - (byte*) pThis));
                    }
                    catch (OverflowException) {
                        return false;
                    }
                    var streamName = pStream->GetNameAsString(maxLength);
                    StreamID streamID;
                    if (! StreamHeader.StreamNames.TryGetValue(streamName, out streamID)) {
                        return false;
                    }
                    if (streams[(int)streamID] != null) {
                        return false;
                    }
                    streams[(int) streamID] = pStream;
                    try {
                        pStream = (StreamHeader *)checked((byte*) pStream + 8 + checked((uint)streamName.Length + 1).Pad(4));
                    }
                    catch (OverflowException) {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool VerifyVersion()
        {
            var version = GetVersion();
            if (version.StartsWith("v4.0.")) {
                return true;
            }

            if (version.StartsWith("Standard CLI ")) {
                var versionNumStr = version.Substring("Standard CLI ".Length);
                int versionNum;
                if (int.TryParse(versionNumStr, out versionNum)) {
                    return versionNum >= 2005 && versionNum <= 2010;
                }
            }

            return false;
        }
    }
}
