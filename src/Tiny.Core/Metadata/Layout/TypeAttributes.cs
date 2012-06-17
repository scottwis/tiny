// TypeAttributes.cs
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
    //# Defines the know flags stored in the [TypeDefRow.Flags][Flags] field in a row in the [MetadataType.TypeDef][TypeDef] table.
    [Flags]
    enum TypeAttributes : uint
    {
        //===========================================================
        //# A mask used to retreive accessibility information.
        VisibilityMask = 0x00000007,
        //# Type type has no public scope.
        NotPublic = 0x00000000,
        //# The type is a top-level type and has public scope.
        Public = 0x00000001,
        //# The type is nested within another type and has public scope.
        NestedPublic = 0x00000002,
        //# The type is nested within another type and has private scope.
        NestedPrivate = 0x00000003,
        //# The type has "protected" scope.
        NestedFamily = 0x00000004,
        //# The type is nested and has 'internal' accessibility
        NestedAssembly = 0x00000005,
        //# The type is nested and has 'protected and internal' accessibility.
        NestedFamANDAssem = 0x00000006,
        //# Type type is nested and has 'protected or internal' accessibility.
        NestedFamORAssem = 0x00000007,
        //===========================================================

        //===========================================================
        //# A mask used to retreive layout information.
        LayoutMask = 0x00000018,

        //# The runtime is free to layout class fields in any way it chooses.
        AutoLayout = 0x00000000,

        //# The runtime must latout fields sequentially (in the order specified in metadata),.
        SequentialLayout = 0x00000008,
        ExplicitLayout = 0x00000010,
        //===========================================================

        //===========================================================
        //# A mask used to retreive class semantic information.
        ClassSemanticsMask = 0x00000020,
        //# Type type is a class.
        Class = 0x00000000,
        //# Type is an interface
        Interface = 0x00000020,
        //===========================================================

        //===========================================================
        //# The type is abstract
        Abstract = 0x00000080,
        //# The type is sealed
        Sealed = 0x00000100,
        //# The type has a special name (property accessors, overloaded operators, etc). Generally this is used by
        //# compilers to trigger special semantics based on names when importing an assembly. For example, a static
        //# method named op_true that has the [SpecialName] flag set will be treated by C# as an overloaded true
        //# operator. If the flag is not set, then it will be treated as a normal method named op_true.
        //#
        //# See also: [RTSpecialName]
        SpecialName = 0x00000400,
        //===========================================================

        //===========================================================
        //# The class / interface is imported. I think this has something to do with COM interop.
        Import = 0x00001000,
        //# The PE / COFF spec says:
        //# > Reserved (Class is serializable)
        Serializable = 0x00002000,
        //===========================================================

        //===========================================================
        //# A mask used to retreive string information for native interop.
        StringFormatMask = 0x00030000,
        //# Indicates that strings should be marshaled as ANSI
        AnsiClass = 0x00000000,
        //# Indicates that strings should be marshaled as unicode.
        UnicodeClass = 0x00010000,
        //# Indicates that strings should be automatically interpreted as either unicode or ascii, depening on the OS.
        //# TDC always interprets such strings as unicode.
        AutoClass = 0x00020000,
        //# Indicates that a non-standard encoding is specified by the bits in [CustomStringFormatMask]. The
        //# tiny decompiler will report an error when attempting to translate a class with this bit set to 1.
        CustomFormatClass = 0x00030000,
        //# A mask used to retreive custom string formatting information. The meaning of these two bits is unspecified.
        CustomStringFormatMask = 0x00C00000,
        //===========================================================

        //===========================================================
        //# Indicates that the class can be "initialized" _at any time_, provided that such occurence is guaranteed to
        //# happen before the first access to a static field on the type. Without this flag, the semantics of the CLR
        //# specify that a class _must_ be initialized _immedietly_ before it's first instance is created, it's first
        //# static method is called, or it's first static field is accessed. That is, the default class initialization
        //# order is strictly defined and the runtime has no flexibility in choosing when a class's static constructor
        //# will execute. Implementing the default behavior is expensive. It requires placing checks before every
        //# constructor invocation and every static method call, so it is desirable to use BeforeFieldInit where
        //# possible.
        BeforeFieldInit = 0x00100000,
        //===========================================================

        //===========================================================
        //# Indicates that the runtime treats the method specially, depending on it's name (.ctor, .cctor, etc).
        //# Note that this is distinct from [SpecialName], which has signigicance only to compilers and other tools.
        RTSpecialName = 0x00000800,
        //# The type has declarative security associated with it.
        HasSecurity = 0x00040000,
        //# An entry in the [MetadataTable.ExportedType][exported types] table is a type forwarder.
        IsTypeForwarder = 0x00200000
        //===========================================================
    }
}
