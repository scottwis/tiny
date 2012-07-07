// ArrayMarshalInfo.cs
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

namespace Tiny.Metadata
{
    public class ArrayMarshalInfo : MarshalInfo
    {
        readonly NativeType m_elementType;
        readonly int? m_paramNum;
        readonly int? m_numParams;

        public ArrayMarshalInfo(NativeType elementType, int? paramNum, int? numParams) : base(NativeType.Array)
        {
            m_elementType = elementType;
            m_paramNum = paramNum;
            m_numParams = numParams;
        }

        public NativeType ElementType
        {
            get { return m_elementType; }
        }

        public int? ParameterNumber
        {
            get { return m_paramNum; }
        }

        public int? NumberOfParameters
        {
            get { return m_numParams; }
        }
    }
}
