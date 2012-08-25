// FatMethodHeader.cs
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
    struct FatMethodHeader
    {
        [FieldOffset(0)] readonly ushort m_flagsAndSize;
        [FieldOffset(2)] readonly ushort m_maxStack;
        [FieldOffset(4)] readonly uint m_codeSize;
        [FieldOffset(8)] readonly MetadataToken m_localVarSigToken;

        public MethodHeaderFlags Flags
        {
            get { return (MethodHeaderFlags) (0x0FFF & m_flagsAndSize) ; }
        }

        public int Size
        {
            get
            {
                return ((0xF000 & m_flagsAndSize) >> 10);
            }
        }

        public int MaxStack
        {
            get { return m_maxStack; }
        }

        public int CodeSize
        {
            get { return checked((int)m_codeSize); }
        }

        public MetadataToken LocalVarSigToken
        {
            get { return m_localVarSigToken; }
        }
    }
}
