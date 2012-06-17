// TypeOrMethodDef.cs
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
    struct TypeOrMethodDef : IToken
    {
        readonly uint m_index;

        public TypeOrMethodDef(uint index) : this()
        {
            m_index = index;
        }

        public bool IsNull
        {
            get { return ((m_index & ~0x1U) >> 1) == 0; }
        }

        public MetadataTable Table
        {
            get
            {
                CheckNull();
                switch (m_index & 0x1) {
                    case 0:
                        return MetadataTable.TypeDef;
                    case 1:
                        return MetadataTable.MethodDef;
                    default:
                        throw new InternalErrorException("This code should be unreachable.S");
                }
            }
        }

        public int Index
        {
            get
            {
                CheckNull();
                return ((int)((m_index & ~0x1U) >> 1)) - 1;
            }
        }

        void CheckNull()
        {
            if (IsNull) {
                throw new InvalidOperationException("The index is null");
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IToken);
        }

        public override int GetHashCode()
        {
            if (IsNull) {
                return 0.GetHashCode();
            }
            return m_index.GetHashCode();
        }

        public bool Equals(IToken token)
        {
            if (token == null) {
                return false;
            }
            if (IsNull) {
                return token.IsNull;
            }
            return !token.IsNull && Table == token.Table && Index == token.Index;
        }

        public int CompareTo(IToken other)
        {
            if (other == null) {
                return 1;
            }

            if (IsNull) {
                return other.IsNull ? 0 : -1;
            }

            if (other.IsNull) {
                return 1;
            }

            var ret = Table.CompareTo(other.Table);
            if (ret == 0) {
                ret = Index.CompareTo(other.Index);
            }
            return ret;
        }
    }
}
