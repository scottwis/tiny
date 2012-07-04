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
using System.Linq;
using System.Runtime.InteropServices;
using BclExtras.Collections;

namespace Tiny.Metadata.Layout
{
    class SignatureParser : IDisposable
    {
        const byte FIELD = 0x6;
        const byte HASTHIS = 0x20;
        const byte EXPLICIT_THIS = 0x40;
        
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

        int ReadInt()
        {
            //TODO: Implement this
            throw new NotImplementedException();
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
                case ElementType.ByRef:
                    return new ModifiedType(TypeKind.ByRef, null, ParseType());
                case ElementType.TypedByRef:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.TypedByRef);
                case ElementType.Void:
                    return m_module.Assembly.Project.GetWellKnownType(WellKnownTypeID.Void);
                case ElementType.Pinned:
                    return new ModifiedType(TypeKind.Pinned, null, ParseType());
                default:
                    //TODO: Finish implementing  this
                    throw new NotImplementedException();
            }
        }

        Type ParseModifier(TypeKind kind)
        {
            return new ModifiedType(
                kind, 
                ParseTypeReference(),
                ParseType()
            );
        }

        Type ParseGenericTypeParameter()
        {
            if (!(m_genericParameterScope is TypeDefinition)) {
                throw new InvalidOperationException("Generic method parameter not expected!");
            }
            return m_genericParameterScope.GenericParameters[checked((int)ReadUInt())];
        }

        Type ParseGenericMethodParameter()
        {
            if (!(m_genericParameterScope is MethodDefinition)) {
                throw new InvalidOperationException("Generic method parameter not expected!");
            }
            return m_genericParameterScope.GenericParameters[checked((int) ReadUInt())];
        }

        Type ParseGenericInstanceType()
        {
            var baseType = ParseType();
            var argCount = ReadUInt();
            var args = new List<Type>();
            argCount.Times(()=>args.Add(ParseType()));
            return new GenericInstanceType(baseType, args.AsReadOnly());
        }

        Type ParseTypeReference()
        {
            return new TypeReference(new TypeDefOrRef(new OneBasedIndex(ReadUInt())),m_module);
        }

        Type ParseArrayType()
        {
            var baseType = ParseType();
            var rank = ReadUInt();
            var sizes = ParseArraySizes();
            var lowerBounds = ParseArrayLowerBounds();
            var dimensions = sizes.Select(x => new uint?(x))
                .ZipExtend(lowerBounds.Select(x => new int?(x)))
                .Select(x => CreateDimension(x.First, x.Second))
                .ZipExtend(Enumerable.Repeat(0, checked((int) rank)))
                .Select(x => x.First ?? new Dimension())
                .ToList().AsReadOnly();
            return new ArrayType(baseType, dimensions);
        }

        Dimension? CreateDimension(uint? size, int? lowerBound)
        {
            if (size == null && lowerBound == null) {
                return new Dimension();
            }
            //NOTE: This relies on null propagation to work correctly. That is,
            //if size is null, then lowerBound + size - 1 will yield null, even if lowerBound is not null.
            return new Dimension(lowerBound ?? 0, lowerBound + checked((int?)size) - 1);
        }

        int[] ParseArrayLowerBounds()
        {
            var count = ReadUInt();
            var ret = new int[count];
            for (int i = 0; i < ret.Length; ++i) {
                ret[i] = ReadInt();
            }
            return ret;
        }

        uint[] ParseArraySizes()
        {
            var count = ReadUInt();
            var ret = new uint[count];
            for (int i = 0; i < ret.Length; ++i) {
                ret[i] = ReadUInt();
            }
            return ret;
        }

        Method ParseMethodSignature()
        {
            var b = ReadByte();
            var hasThis = (b & HASTHIS) != 0;
            var explicitThis = (b & EXPLICIT_THIS) != 0;
            var callingConvention = (CallingConvention) (b & 0x1F);
            var genericParamCount = callingConvention == CallingConvention.Generic ? checked((int)ReadUInt()) : 0;
            var parameters = new List<Parameter>(checked((int)ReadUInt()));
            var retType = ParseType();
            for (var i = 0; i < parameters.Count; ++i) {
                parameters.Add(ParseParameter());
            }

            return new MethodSignature(hasThis, explicitThis, callingConvention, genericParamCount, parameters.AsReadOnly(), retType);
        }

        Parameter ParseParameter()
        {
            return new Parameter(ParseType());
        }

        public void Dispose()
        {
            m_enumerator.Dispose();
        }
    }
}
