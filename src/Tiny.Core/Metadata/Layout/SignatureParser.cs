// SignatureParser.cs
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

namespace Tiny.Metadata.Layout
{
    class SignatureParser : IDisposable
    {
        const byte FIELD = 0x6;

        readonly IEnumerator<byte> m_enumerator;
        readonly Module m_module;
        readonly IGenericParameterScope m_genericParameterScope;

        SignatureParser(IReadOnlyList<byte> signature, IGenericParameterScope scope)
        {
            m_enumerator = signature.CheckNotNull("signature").GetEnumerator();
            m_genericParameterScope = scope.CheckNotNull("scope");
            m_module = scope.Module;
        }

        public static Type ParseFieldSignature(IReadOnlyList<byte> signature, TypeDefinition declaringType)
        {
            using (var parser = new SignatureParser(signature, declaringType)) {
                return parser.ParseFieldSignature();
            }
        }

        Type ParseFieldSignature()
        {
            ReadByte(FIELD);
            return ParseType();
        }

        byte ReadByte()
        {
            m_enumerator.MoveNext().Assume("Unexpected end of signature.");
            return m_enumerator.Current;
        }

        byte ReadByte(byte value)
        {
            return ReadByte().AssumeEQ(value, "Unexpected value when reading signature");
        }

        uint ReadUInt()
        {
            var b0 = ReadByte();
            if ((b0 & 0x80) == 0x00) {
                return b0;
            }
            if ((b0 & 0xC0) == 0x80) {
                return ((b0 & ~0x80U) << 8) | ReadByte();
            }
            return
                ((b0 & ~0xC0U) << 24)
                | (((uint)ReadByte()) << 16)
                | (((uint)ReadByte()) << 8)
                | ((uint)ReadByte());
        }

        Type ParseType()
        {
            var x = ReadUInt();
            switch ((ElementType)x) {
                case ElementType.Boolean:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Boolean);
                case ElementType.Char:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Character);
                case ElementType.I1:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.SByte);
                case ElementType.U1:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Byte);
                case ElementType.I2:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Int16);
                case ElementType.U2:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.UInt16);
                case ElementType.I4:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Int32);
                case ElementType.U4:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.UInt32);
                case ElementType.I8:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Int64);
                case ElementType.U8:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.UInt64);
                case ElementType.R4:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Single);
                case ElementType.R8:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Double);
                case ElementType.I:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.IntPtr);
                case ElementType.U:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.UIntPtr);
                case ElementType.Array:
                    return ParseArrayType();
                case ElementType.Class:
                    return ParseTypeReference();
                case ElementType.FunctionPointer:
                    return new FunctionPointerType(ParseMethodSignature());
                case ElementType.GenericInstantiaton:
                    return ParseGenericInstanceType();
                case ElementType.MethodVariable:
                    return ParseGenericMethodParameter();
                case ElementType.Object:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Object);
                case ElementType.Pointer:
                    return new ModifiedType(TypeKind.Pointer, null, ParseType());
                case ElementType.String:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.String);
                case ElementType.Vector:
                    return new ModifiedType(TypeKind.Vector, null, ParseType());
                case ElementType.ValueType:
                    return ParseTypeReference();
                case ElementType.TypeVariable:
                    return ParseGenericTypeParameter();
                case ElementType.ModOpt:
                    return ParseModifier(TypeKind.ModOpt);
                case ElementType.ModReq:
                    return ParseModifier(TypeKind.ModReq);
                default:
                    //TODO: Finish implementing  this
                    throw new NotImplementedException();
            }
        }

        Type ParseModifier(TypeKind modOpt)
        {
            throw new NotImplementedException();
        }

        Type ParseGenericTypeParameter()
        {
            throw new NotImplementedException();
        }

        Type ParseGenericMethodParameter()
        {
            throw new NotImplementedException();
        }

        Type ParseGenericInstanceType()
        {
            throw new NotImplementedException();
        }

        MethodReference ParseMethodSignature()
        {
            throw new NotImplementedException();
        }

        Type ParseTypeReference()
        {
            return new TypeReference(new TypeDefOrRef(ReadUInt()),m_module);
        }

        Type ParseArrayType()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            m_enumerator.Dispose();
        }
    }
}
