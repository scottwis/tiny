// GenericParameterRow.cs
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

namespace Tiny.Metadata.Layout
{
    [StructLayout(LayoutKind.Explicit)]
    unsafe struct GenericParameterRow
    {
        [FieldOffset(0)] public readonly ushort Ordinal;
        [FieldOffset(2)] public readonly GenericParameterAttributes Flags;

        public TypeOrMethodDef GetOwner(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (GenericParameterRow* pThis = &this) {
                var pOwner = (byte*) pThis + 4;
                uint index;
                if (CodedIndex.TypeOrMethodDef.IndexSize(peFile) == 2) {
                    index = *(ushort*) pOwner;
                }
                else {
                    index = *(uint*) pOwner;
                }
                return new TypeOrMethodDef(new OneBasedIndex(index));
            }
        }

        public uint GetNameOffset(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (GenericParameterRow* pThis = &this) {
                var pOwner = (byte*) pThis + 4 + CodedIndex.TypeOrMethodDef.IndexSize(peFile);
                if (StreamID.Strings.IndexSize(peFile) == 2) {
                    return *(ushort*) pOwner;
                }
                return *(uint*) pOwner;
            }
        }
    }
}
