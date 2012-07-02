// Event.cs
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
using System.Text;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    public unsafe class Event : IMemberDefinitionInternal
    {
        internal Event(EventRow* pRow, TypeDefinition declaringType)
        {
            //TODO: Implement this
            throw new NotImplementedException();
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public string FullName
        {
            get { throw new NotImplementedException(); }
        }

        public TypeDefinition DeclaringType
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPublic
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPrivate
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsProtected
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsInternal
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsInternalOrProtected
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsInternalAndProtected
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAbstract
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSealed
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasSpecialName
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasRuntimeSpecialName
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsCompilerControlled
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsStatic
        {
            get { throw new NotImplementedException(); }
        }

        void IMemberDefinitionInternal.GetFullName(StringBuilder b)
        {
            throw new NotImplementedException();
        }

        public MethodDefinition AddMethod
        {
            get { throw new NotImplementedException(); }
        }

        public MethodDefinition RemoveMethod
        {
            get { throw new NotImplementedException(); }
        }

        public MethodDefinition FireMethod
        {
            get { throw new NotImplementedException(); }
        }
    }
}
