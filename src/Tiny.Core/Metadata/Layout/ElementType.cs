// ElementType.cs
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
    enum ElementType : uint
    {
        End = 0x00,
        Void = 0x01,
        Boolean = 0x02,
        Char = 0x03,
        I1 = 0x04,
        U1 = 0x05,
        I2 = 0x06,
        U2 = 0x07,
        I4 = 0x08,
        U4 = 0x09,
        I8 = 0x0a,
        U8 = 0x0b,
        R4 = 0x0c,
        R8 = 0x0d,
        String = 0x0e,
        Pointer = 0x0f,
        ByRef = 0x10,
        ValueType = 0x11,
        Class = 0x12,
        TypeVariable = 0x13,
        Array = 0x14,
        GenericInstantiaton=0x15,
        TypedByRef = 0x16,
        I = 0x18,
        U = 0x19,
        FunctionPointer = 0x1b,
        Object = 0x1c,
        Vector = 0x1d,
        MethodVariable = 0x1e,
        ModReq = 0x1f,
        ModOpt = 0x20,
        Internal = 0x21,
        /*Modfier = 0x40,*/
        Sentinel = 0x41,
        Pinned = 0x45,
        SystemType = 0x50,
        Boxed = 0x51,
        ReservedFlag = 0x52,
        Field = 0x53,
        Property = 0x54,
        Emum = 0x55
    }
}
