// TypeKind.cs
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

namespace Tiny.Metadata
{
    public enum TypeKind
    {
        TypeDefinition,
        ModOpt,
        ModReq,
        GeneralArray,
        Vector,
        FunctionPointer,
        GenericInstance,
        GenericParameter,
        Pointer,
        ByRef,
        Pinned,
        Sentinel,
        TypeReference
    }

    public static class TypeKindExtensions
    {
        public static bool IsModifiedType(this TypeKind kind)
        {
            switch (kind) {
                case TypeKind.ModOpt:
                case TypeKind.ModReq:
                case TypeKind.Sentinel:
                case TypeKind.Pinned:
                case TypeKind.Pointer:
                case TypeKind.ByRef:
                case TypeKind.Vector:
                    return true;
                default:
                    return false;
            }
        }

        public static string ModifierName(this TypeKind kind)
        {
            switch (kind) {
                case TypeKind.ModOpt:
                    return "modopt";
                case TypeKind.ModReq:
                    return "modreq";
                case TypeKind.Pinned:
                    return "pinned";
                case TypeKind.Sentinel:
                    return "sentinel";
                default:
                    throw new ArgumentException("Not a valid modifier kind.", "kind");
            }
        }

        public static string Suffix(this TypeKind kind)
        {
            switch (kind) {
                case TypeKind.Pointer:
                    return "*";
                case TypeKind.ByRef:
                    return "&";
                case TypeKind.Vector:
                    return "[]";
                default:
                    throw new ArgumentException("Kind does not have a suffix.", "kind");
            }
        }

        public static bool IsPointerOrByRef(this TypeKind kind)
        {
            switch (kind) {
                case TypeKind.Pointer:
                case TypeKind.ByRef:
                    return true;
                default:
                    return false;
            }
        }
    }
}
