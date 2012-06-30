// MethodImplAttributes.cs
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
    [Flags]
    enum MethodImplAttributes : uint
    {
        //============================================================
        //# A mask used to extract the method type
        CodeTypeMask = 0x0003,
        //# Indicates that a method is implemented in IL
        IL = 0x0000,
        //# Indicates that a method is implemented in native code
        Native = 0x0001,
        //# The ECMA spec simply says:
        //# > Reserved: Shall be 0 in conforming implementations.
        //# I'm not sure what it's used for.
        OPTIL = 0x0002,
        //# Indicates that a method is implemented internally by the CLR
        Runtime = 0x0003,
        //============================================================

        //# Indicates that a mehtod is unmanaged. I'm not sure how this is different from the "Native" CodeType.
        Unmanaged = 0x0004,

        //============================================================
        //# Not sure. The ECMA Spec says:
        //# > Indicates method is defined; used primaruly in merge scenarios.
        ForwardRef = 0x0010,
        //# The ECMA Spec says:
        //# > Reserved: conforming implementations \[of the clr\] can ignore.
        PreserveSig = 0x0080,
        //# 
        InternalCall = 0x1000,
        Synchronized = 0x0020,
        NoInlining = 0x0008,
        MaxMethodImplVal = 0xffff,
        NoOptimization = 0x0040
        //============================================================
    }
}