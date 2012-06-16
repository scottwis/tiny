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

using System;
using System.Text;

namespace Tiny.Metadata
{
    //# Represents a type that modfies another type. This includes:
    //# 1. [TypeKind.ModOpt]
    //# 2. [TypeKind.ModReq]
    //# 3. [TypeKind.Sentinel]
    //# 4. [TypeKind.Pinned]
    //# 5. [TypeKind.Pointer]
    //# 6. [TypeKind.ByRef]
    //# 7. [TypeKind.Vector]
    public sealed class ModifiedType : Type
    {
        readonly Type m_modfier;
        readonly Type m_baseType;

        internal ModifiedType(TypeKind kind, Type modifier, Type baseType) : 
            base(kind.Check(kind.IsModifiedType(), "Not a valid modified type kind", "kind"))
        {
            if (Kind == TypeKind.ModOpt || Kind == TypeKind.ModReq) {
                m_modfier = modifier.CheckNotNull("modifier");
            }
            else {
                m_modfier = modifier.CheckNull("modifier", "A modifier type may only be specified for ModOpt and ModReq types");
            }
            
            m_baseType = baseType.CheckNotNull("baseType");
        }

        //# For [TypeKind.ModOpt] or [TypeKind.ModReq] types, returns the modifier type being applied to [BaseType].
        //# Otherwise, null is returned.
        public Type Modifier
        {
            get { return m_modfier; }
        }

        //# The base type being modified.
        public Type BaseType
        {
            get { return m_baseType; }
        }

        internal override void GetFullName(StringBuilder b)
        {
            switch (Kind) {
                case TypeKind.ModOpt:
                case TypeKind.ModReq:
                case TypeKind.Pinned:
                case TypeKind.Sentinel:
                    b.Append(Kind.ModifierName());
                    b.Append("(");
                    Modifier.GetFullName(b);
                    b.Append(") ");
                    BaseType.GetFullName(b);
                    break;
                case TypeKind.Pointer:
                case TypeKind.ByRef:
                case TypeKind.Vector:
                    BaseType.GetFullName(b);
                    if (BaseType.Kind.IsPointerOrByRef()) {
                        b.Append(" ");
                    }
                    b.Append(Kind.Suffix());
                    break;
                default:
                    throw new InternalErrorException("Unexpected type kind");
            }
            
        }
    }
}
