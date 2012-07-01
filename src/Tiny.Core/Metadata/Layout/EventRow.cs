// EventRow.cs
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
    unsafe struct EventRow
    {
        [FieldOffset(0)]
        public readonly EventAttributes EventFlags;

        public uint GetNameIndex(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (EventRow * pThis = &this) {
                var pName = (byte*) pThis + 2;
                if (StreamID.Strings.IndexSize(peFile) == 2) {
                    return *(ushort*) pName;
                }
                return *(uint*) pName;
            }
        }

        public TypeDefOrRef GetEventType(PEFile peFile)
        {
            peFile.CheckNotNull("peFile");
            fixed (EventRow * pThis = &this) {
                var pType = (byte*) pThis + 2 + StreamID.Strings.IndexSize(peFile);
                uint index;
                if (CodedIndex.TypeDefOrRef.IndexSize(peFile) == 2) {
                    index = *(ushort*) pType;
                }
                else {
                    index = *(uint*) pType;
                }
                return new TypeDefOrRef(index);
            }
        }
    }
}
