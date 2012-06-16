// GenericParameterAttributes.cs
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
    //# Defines the bit values used for the [GenericParameterRow.Flags] field in a row
    //# in the generic parameter table in a managed PE file.
    [Flags]
    enum GenericParameterAttributes : ushort
    {
        //===========================================================
        //# A mask used to extract generic variance information
        VarianceMask = 0x0003,
        //# Indicates that a generic parameter is non-variant.
        None = 0x0000,
        //# Indicates that a generic parameter is co-variant.
        Covariant = 0x0001,
        //# Indicates that a generic parameter is contra-variant.
        Contravariant = 0x0002,
        //===========================================================

        //===========================================================
        //# A mask used to extract constraint information.
        SpecialConstraintMask = 0x001c,
        //# Indicates that a generic parameter has a 'class' constraint.
        ReferenceTypeConstraint = 0x0004,
        //# Indicates that a generic parameter has a 'struct' constraint.
        NotNullableValueTypeConstraint = 0x0008,
        //# Indicates that a generic parameter has a 'new()' constraint.
        DefaultConstructorConstraint = 0x0010,
        //===========================================================
    }
}
