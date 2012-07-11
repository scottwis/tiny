// Parameter.cs
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
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    public unsafe class Parameter
    {
        internal Parameter(Type type)
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

        internal Parameter(ParamRow* pRow, Type returnType)
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

        internal Parameter(ParamRow* pRow, Parameter signatureParam)
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

        //TODO: Finish defining this class

        public Type ParameterType
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public IReadOnlyList<CustomAttribute> CustomAttributes
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public MarshalInfo MarshalInfo
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        void CheckDisposed()
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

    }
}
