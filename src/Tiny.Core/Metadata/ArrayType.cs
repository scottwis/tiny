// ArrayType.cs
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
using System.Collections.Generic;
using System.Text;

namespace Tiny.Metadata
{
    //# Represents a "general array type" in the CLR. Note that this is distinct from a "vector type"
    //# which is used to represent zero-based single dimensional arrays. All instances of ArrayType
    //# will either represent a multi-dimensional array or an array type with a non zero lower bound.
    class ArrayType : Type
    {
        readonly Type m_baseType;
        readonly IReadOnlyList<Dimension> m_dimensions;

        internal ArrayType(Type baseType, IReadOnlyList<Dimension> dimensions) : base(TypeKind.GeneralArray)
        {
            m_baseType = baseType.AssumeNotNull();
            m_dimensions = dimensions.AssumeNotNull();
            if (m_dimensions.Count == 0) {
                throw new ArgumentException("The number of dimensions specified cannot be zero.");
            }
        }

        //# Returns the rank of the array type.
        public int Rank
        {
            get { return m_dimensions.Count; }
        }

        public Type BaseType
        {
            get { return m_baseType; }
        }

        //# Returns the set of dimension specifications for the array type. Unlike the underlying meta-data, this list
        //# will contain one entry for each dimension. Unspecified dimensions will have nulls for both their upper and lower bound values.
        public IReadOnlyList<Dimension> Dimensions
        {
            get { return m_dimensions; }
        }

        internal override void GetFullName(StringBuilder b)
        {
            BaseType.GetFullName(b);
            b.Append("[");

            var first = true;
            foreach (var dimension in Dimensions) {
                if (first) {
                    first = false;
                }
                else {
                    b.Append(", ");
                }
                dimension.GetFullName(b);
            }
            b.Append("]");
        }
    }
}
