// FieldDefinition.cs
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
using System.Text;
using System.Threading;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    public unsafe sealed class FieldDefinition : IMemberDefinition
    {
        volatile string m_name;
        readonly FieldRow * m_pRow;
        readonly TypeDefinition m_declaringType;
        volatile Type m_fieldType;

        internal FieldDefinition(FieldRow * pRow, TypeDefinition declaringType)
        {
            m_pRow = (FieldRow *)FluentAsserts.CheckNotNull((void*) pRow, "pRow");
            m_declaringType = declaringType.CheckNotNull("declaringType");
        }

        public string Name
        {
            get
            {
                CheckDisposed();
                if (m_name == null) {
                    var name = DeclaringType.Module.PEFile.ReadSystemString(
                        m_pRow->GetNameOffset(DeclaringType.Module.PEFile)
                    );
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_name, name, null);
                    #pragma warning restore 420
                }
                return m_name;
            }
        }

        public string FullName
        {
            get
            {
                CheckDisposed();
                var b = new StringBuilder();
                DeclaringType.GetFullName(b);
                b.AppendFormat(".{0}", Name);
                return b.ToString();
            }
        }

        public TypeDefinition DeclaringType
        {
            get
            {
                CheckDisposed();
                return m_declaringType;
            }
        }

        public Type FieldType
        {
            get
            {
                CheckDisposed();
                if (m_fieldType == null ) {
                    var type =DeclaringType.Module.PEFile.ParseFieldSignature(
                        m_pRow->GetSignatureOffset(DeclaringType.Module.PEFile),
                        DeclaringType
                    );
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_fieldType, type, null);
                    #pragma warning restore 420
                }
                return m_fieldType;
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

        public bool IsPublic
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool IsPrivate
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool IsProtected
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool IsInternalAndProtected
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool IsInternalOrProtected
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool IsInternal
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool HasSpecialName
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool HasRuntimeSpecialName
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool IsCompilerControlled
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        bool IMemberDefinition.IsAbstract
        {
            get
            {
                CheckDisposed();
                return false;
            }
        }

        bool IMemberDefinition.IsSealed
        {
            get
            {
                CheckDisposed();
                return false;
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

        public IReadOnlyList<byte> InitialValue
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool IsStatic
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public uint RVA
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        private void CheckDisposed()
        {
            if (m_declaringType.Module.PEFile.IsDisposed) {
                throw new ObjectDisposedException("FieldDefinition");
            }
        }
    }
}
