// TypeDefRow.cs
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
    unsafe struct TypeDefRow
    {
        [FieldOffset(0)] public readonly TypeAttributes Flags;

        public uint GetTypeNameIndex(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (TypeDefRow* pThis = &this) {
                var pName = checked((byte*) pThis + 4);
                if (StreamID.Strings.IndexSize(peFile) == 2) {
                    return *(ushort*) pName;
                }
                return *(uint*) pName;
            }
        }

        public uint GetTypeNamespaceIndex(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (TypeDefRow* pThis = &this) {
                var pNamespace = checked((byte*) pThis + 4 + StreamID.Strings.IndexSize(peFile));
                if (StreamID.Strings.IndexSize(peFile)== 2) {
                    return *(ushort*) pNamespace;
                }
                return *(uint*) pNamespace;
            }
        }

        public TypeDefOrRef GetExtendsToken(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (TypeDefRow* pThis = &this) {
                var pExtends = checked((byte*) pThis + 4 + 2*StreamID.Strings.IndexSize(peFile));
                uint index;
                if (CodedIndex.TypeDefOrRef.IndexSize(peFile) == 2) {
                    index = *(ushort*) pExtends;
                }
                else {
                    index = *(uint*) pExtends;
                }
                return new TypeDefOrRef(index);
            }
        }

        public uint GetFieldListToken(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (TypeDefRow* pThis = &this) {
                var pFieldList = checked(
                    (byte*) pThis 
                    + 4 
                    + 2*StreamID.Strings.IndexSize(peFile)
                    + CodedIndex.TypeDefOrRef.IndexSize(peFile)
                );
                if (MetadataTable.Field.IndexSize(peFile) == 2) {
                    return *(ushort*) pFieldList;
                }
                return *(uint*) pFieldList;
            }
        }

        public uint GetMethodListToken(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (TypeDefRow* pThis = &this) {
                var pFieldList = checked(
                    (byte*) pThis 
                    + 4 
                    + 2*StreamID.Strings.IndexSize(peFile)
                    + CodedIndex.TypeDefOrRef.IndexSize(peFile)
                );
                if (MetadataTable.MethodDef.IndexSize(peFile) == 2) {
                    return *(ushort*) pFieldList;
                }
                return *(uint*) pFieldList;
            }
        }
    }
}
