// MetadataToken.cs
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

namespace Tiny.Metadata.Layout
{
    //# A generic implementation of [IToken] that can encode a reference to any valid meta-data table.
    //# The most-signifigant byte is used to decode the associated table.
    struct MetadataToken : IToken
    {
        readonly uint m_value;

        public MetadataToken(uint value)
        {
            m_value = value;
        }

        public bool IsNull
        {
            get { return (0x00FFFFFF & m_value) == 0; }
        }

        public MetadataTable Table
        {
            get
            {
                NullCheck();
                return (MetadataTable)(((0xFF000000) & m_value) >> 24).AssumeDefined("Invalid meta-data table.");
            }
        }

        public uint Index
        {
            get
            {
                NullCheck();
                return ((m_value) & 0x00FFFFFF) - 1;
            }
        }

        private void NullCheck()
        {
            if (IsNull) {
                throw new InvalidOperationException("The token is null");
            }
        }
    }
}
