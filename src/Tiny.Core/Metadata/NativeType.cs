// NativeType.cs
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
    //# Defines an enum representing the "NATIVE_TYPE" constants used in marshaling descriptors.
    //# Reference: Ecma-335 Spec, 5th Edtion, Partition II § 23.4
    public enum NativeType
    {
        Boolean = 0x02,
        I1 = 0x03,
        U1 = 0x04,
        I2 = 0x05,
        U2 = 0x06,
        I4 = 0x07,
        U4 = 0x08,
        I8 = 0x09,
        U8 = 0x0a,
        R4 = 0x0b,
        R8 = 0x0c,
        LpStr = 0x14,
        LpWStr = 0x15,
        Int = 0x1f,
        Uint = 0x20,
        Func = 0x26,
        Array = 0x2a
    }

    public static class NativeTypeExtensions
    {
        public static bool IsIntrinsic(this NativeType nativeType)
        {
            switch (nativeType) {
                case NativeType.Boolean:
                case NativeType.I1:
                case NativeType.U1:
                case NativeType.I2:
                case NativeType.U2:
                case NativeType.I4:
                case NativeType.U4:
                case NativeType.I8:
                case NativeType.U8:
                case NativeType.R4:
                case NativeType.R8:
                case NativeType.LpStr:
                case NativeType.LpWStr:
                case NativeType.Int:
                case NativeType.Uint:
                case NativeType.Func:
                    return true;
                default:
                    return false;
            }
        }
    }
}
