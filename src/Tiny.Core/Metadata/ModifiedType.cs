// ModifiedType.cs
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

using System.Text;

namespace Tiny.Metadata
{
    //# Represents either a modopt or modreq type. A ModOpt or ModReq will "modify" a type by applying another type
    //# as a modifier. For example, the Managed C++ compiler will use
    //# "modopt(System.Runtime.CompilerServices.IsConst) T" to represent "const T" types, allowing Managed C++ classes
    //# to overload methods between "const T" and "T".
    public class ModifiedType : Type
    {
        readonly Type m_modfier;
        readonly Type m_baseType;
        readonly bool m_isRequired;

        internal ModifiedType(Type modifier, Type baseType, bool isRequred)
        {
            m_modfier = modifier.CheckNotNull("modifier");
            m_baseType = baseType.CheckNotNull("baseType");
            m_isRequired = isRequred;
        }

        //# The type of the modifier being applied.
        public Type Modifier
        {
            get { return m_modfier; }
        }

        //# The type [Modifier] applies to.
        public Type BaseType
        {
            get { return m_baseType; }
        }

        //# Indicates wether they modifier is a "required-modifier" (ModReq) or an "optional modifier" (ModOpt)
        public bool IsRequired
        {
            get { return m_isRequired; }
        }

        internal override void GetFullName(StringBuilder b)
        {
            b.Append(IsRequired ? "modreq" : "modopt");
            b.Append("(");
            Modifier.GetFullName(b);
            b.Append(") ");
            BaseType.GetFullName(b);
        }
    }
}
