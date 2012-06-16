// GenericInstanceType.cs
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

using System.Collections.Generic;
using System.Text;

namespace Tiny.Metadata
{
    //# Represents an instanstation of a generic type, such as IEnumerable<Sting> or
    //# List<int>.
    public sealed class GenericInstanceType : Type
    {
        readonly TypeDefinition m_baseType;
        readonly IReadOnlyList<Type> m_parameters;

        public GenericInstanceType(TypeDefinition baseType, IReadOnlyList<Type> parameters) : base(TypeKind.GenericInstance)
        {
            m_baseType = baseType.CheckNotNull("baseType");
            m_parameters = parameters.CheckNotNull("parameters");
        }

        public TypeDefinition BaseType
        {
            get { return m_baseType; }
        }

        public IReadOnlyList<Type> Parameters
        {
            get { return m_parameters; }
        }

        internal override void GetFullName(StringBuilder b)
        {
            BaseType.GetFullName(b);
            b.Append("<");
            var first = true;
            foreach (var p in Parameters) {
                if (first) {
                    first = false;
                }
                else {
                    b.Append(",");
                }
                p.GetFullName(b);
            }
            b.Append(">");
        }
    }
}
