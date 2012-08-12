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
        int m_readCount;

        SignatureParser(IReadOnlyList<byte> signature, IGenericParameterScope scope)
        {
            m_enumerator = signature.CheckNotNull("signature").GetEnumerator();
            m_genericParameterScope = scope.CheckNotNull("scope");
            m_module = scope.Module;
        }

        SignatureParser(IReadOnlyList<byte> signature)
        {
            m_enumerator = signature.CheckNotNull("signature").GetEnumerator();
            m_module = null;
            m_genericParameterScope = null;
        }

        public static Type ParseFieldSignature(IReadOnlyList<byte> signature, TypeDefinition declaringType)
        {
            using (var parser = new SignatureParser(signature, declaringType)) {
                return parser.ParseFieldSignature();
            }
        }

        public static Method ParseMethodSignature(IReadOnlyList<byte> signature, MethodDefinition targetMethod)
        {
            using (var parser = new SignatureParser(signature, targetMethod)) {
                return parser.ParseMethodSignature();
            }
        }

        public static MarshalInfo ParseMarshalDescriptor(IReadOnlyList<byte> signature)
        {
            using (var parser = new SignatureParser(signature)) {
                return parser.ParseMashalDescriptor();
            }
        }

        public static uint ParseLength(IReadOnlyList<byte> blob, out int bytesRead)
        {
            using (var parser = new SignatureParser(blob)) {
                var ret = parser.ReadUInt();
                bytesRead = parser.m_readCount;
                return ret;
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
            checked {
                ++m_readCount;
            }
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

        uint? TryReadUInt()
        {
            if (m_enumerator.MoveNext()) {
                checked {
                    ++m_readCount;
                }
                var b0 = m_enumerator.Current;
                if ((b0 & 0x80) == 0x00) {
                    return b0;
                }
                if ((b0 & 0xC0) == 0x80) {
                    return ((b0 & ~0x80U) << 8) | ReadByte();
                }
                return
                    ((b0 & ~0xC0U) << 24)
                    | (((uint) ReadByte()) << 16)
                    | (((uint) ReadByte()) << 8)
                    | ((uint) ReadByte());
            }
            return null;
        }

        int ReadInt()
        {
            /*var b0 = ReadByte();
            if ((b0 & 0x80) == 0x00) {
                //Treating b0 as a 7 bit integer, rotate it to the right by 1 position.
                //C# doesn't have a rotate operator, so we simulate it using a right shift and an or.
                var tmp = (b0 >> 1) | (b0 & 1) << 6;
                //Then coy bit 6 into bit 7 (sign extend 7 bits to 8 bits), and then finally sign extend 1 byte to 4 bytes.
                return (sbyte) (tmp | ((b0 & 1) << 7));
            }
            if ((b0 & 0xC0) == 0x80) {
                var b1 = ReadByte();
                //Reconstruct the 14 bit value stored in b0 and b1 by combining them into 2 bytes and rotating
                //right by 1 bit.
                var combinedResults = (((b0 & ~0x80) << 8 | b1) >> 1) & ((b1 & 1) << 13);
                //Sign extend the result to 32 bits, first by sign extending the 14 bit result to a 16 bit result
                //(by copying bit 13 to bits 14 and 15), and then sign extend the 16 bit result to a 32 bit result
                //(by doing a cast). 
                return (short) (ushort) (combinedResults | ((b1 & 1) << 14) | ((b1 & 1) << 15));
            }
            else {
                var b1 = ReadByte();
                var b2 = ReadByte();
                var b3 = ReadByte();

                //Reconstruct the 29 bit value stored in b0, b1, b2, and b3, by combining them into 4 bytes
                //and rotating right by 1 bit.
                var combinedResults = ((
                    ((b0 & ~0xC0U) << 24)
                    | (b1 << 16)
                    | (b2 << 8)
                    | b3
                ) >> 1) | ((b3 & 1) << 28);

                //Sign extend the result from 29 bits to 32 bits by copying bit 28 to bits 29, 30, and 31.
                return (int)(uint)(combinedResults | ((b3 & 1) << 29) | ((b3 & 1) << 30) | ((b3 & 1) << 31));
            }*/
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
            if ((m_genericParameterScope is MethodDefinition)) {
                return m_genericParameterScope.DeclaringType.GenericParameters[checked((int) ReadUInt())];
            }
            if (m_genericParameterScope is TypeDefinition) {
                return m_genericParameterScope.GenericParameters[checked((int)ReadUInt())];
            }
            throw new InvalidOperationException("Generic type parameter not expected.");
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

        MarshalInfo ParseMashalDescriptor()
        {
            var b = ReadByte();
            if (b == (int)NativeType.Array) {
                var elementType = ReadByte();
                if (((NativeType)elementType).IsIntrinsic()) {
                    var paramNum = TryReadUInt();
                    var numElements = TryReadUInt();
                    return new ArrayMarshalInfo((NativeType)elementType, checked((int?)paramNum), checked((int?)numElements));
                }
                throw new InvalidOperationException("Unable to parse marhsal descriptor: the array element type is not an intrinsic type.");
            }
            if (((NativeType)b).IsIntrinsic()) {
                return new MarshalInfo((NativeType) b);
            }
            throw new InvalidOperationException("Unable to parse marshal descriptor");
        }

        public void Dispose()
        {
            m_enumerator.Dispose();
        }
    }
}
