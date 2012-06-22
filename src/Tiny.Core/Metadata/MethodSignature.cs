// MethodSignature.cs
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

using System.Collections.Generic;

namespace Tiny.Metadata
{
    class MethodSignature : Method
    {
        readonly IReadOnlyList<Parameter> m_parameters;
        readonly bool m_hasThis;
        readonly bool m_explicitThis;
        readonly CallingConvention m_callingConvention;
        readonly int m_genericParamCount;
        readonly Type m_retType;

        public MethodSignature(
            bool hasThis,
            bool explicitThis,
            CallingConvention callingConvention,
            int genericParamCount,
            IReadOnlyList<Parameter> parameters,
            Type retType
            )
        {
            m_hasThis = hasThis;
            m_explicitThis = explicitThis;
            m_callingConvention = callingConvention.CheckDefined("callingConvention");
            m_genericParamCount = genericParamCount.CheckGTE(0, "genericParamCount");
            m_parameters = parameters.CheckNotNull("parameters");
            m_retType = retType.CheckNotNull("retType");
        }

        public override string Name
        {
            get { return null; }
        }

        public override Type ReturnType
        {
            get { return m_retType; }
        }

        public override IReadOnlyList<Parameter> Parameters
        {
            get { return m_parameters; }
        }

        public override bool HasThis
        {
            get { return m_hasThis; }
        }

        public override bool ExplicitThis
        {
            get { return m_explicitThis; }
        }

        public override CallingConvention CallingConvention
        {
            get { return m_callingConvention; }
        }

        public override int GenericParameterCount
        {
            get { return m_genericParamCount; }
        }
    }
}
