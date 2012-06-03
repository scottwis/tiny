// ModuleRow.cs
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

namespace Tiny.Decompiler.Metadata.Layout
{
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct ModuleRow
    {
        [FieldOffset(0)] public readonly ushort Generation;

        public uint GetNameOffset(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (ModuleRow* pThis = &this) {
                var pName = (byte*) pThis + 2;
                if (StreamID.Strings.IndexSize(peFile) == 2) {
                    return *(ushort*) pName;
                }
                return *(uint*) pName;
            }
        }

        public uint GetMvidOffset(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (ModuleRow* pThis = &this) {
                var pMvid = (byte*) pThis + 2 + StreamID.Strings.IndexSize(peFile);
                if (StreamID.Guid.IndexSize(peFile) == 2) {
                    return *(ushort*) pMvid;
                }
                return *(uint*) pMvid;
            }
        }

        public uint GetEncIdOffset(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (ModuleRow* pThis = &this) {
                var pEncId = (byte*) pThis + 2 + StreamID.Strings.IndexSize(peFile) + StreamID.Guid.IndexSize(peFile);
                if (StreamID.Guid.IndexSize(peFile) == 2) {
                    return *(ushort*) pEncId;
                }
                return *(uint*) pEncId;
            }
        }

        public uint GetEncBaseId(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (ModuleRow* pThis = &this) {
                var pBaseId = (byte*) pThis + 2 + StreamID.Strings.IndexSize(peFile) + StreamID.Guid.IndexSize(peFile)*2;
                if (StreamID.Guid.IndexSize(peFile) == 2) {
                    return *(ushort*) pBaseId;
                }
                return *(uint*) pBaseId;
            }
        }
    }
}
