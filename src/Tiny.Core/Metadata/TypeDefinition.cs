// TypeDefinition.cs
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
using System.Threading;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    //# Represents the definition of a class, struct, interface, or enum type defined in a managed executable.
    public sealed unsafe class TypeDefinition : Type, ITypeContainer, IMutableTypeContainer
    {
        volatile bool m_didSetParent;
        volatile TypeDefinition m_parent;
        readonly TypeDefRow* m_pRow;
        volatile IReadOnlyList<TypeDefinition> m_nestedTypes;
        volatile string m_name;

        internal TypeDefinition(TypeDefRow * pRow, PEFile peFile) : base(peFile)
        {
            FluidAsserts.CheckNotNull((void*)pRow, "pRow");
            m_pRow = pRow;
        }

        public TypeDefinition DeclaringType
        {
            get
            {
                CheckDisposed();
                return m_parent;
            }
            internal set
            {
                CheckDisposed();
                if (! m_didSetParent) {
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_parent, value, null);
                    #pragma warning restore 420
                    m_didSetParent = true;
                }

                if (m_parent != value) {
                    throw new InvalidOperationException("The value of DeclaringType cannot be modified once it has been set.");
                }
            }
        }

        public IReadOnlyList<TypeDefinition> Types
        {
            get
            {
                CheckDisposed();
                return m_nestedTypes;
            }
        }

        IReadOnlyList<TypeDefinition> IMutableTypeContainer.Types
        {
            set
            {
                CheckDisposed();
                value.CheckNotNull("value");
                #pragma warning disable 420
                // ReSharper disable PossibleUnintendedReferenceComparison
                    Interlocked.CompareExchange(ref m_nestedTypes, value, null);

                    if (value != m_nestedTypes) {
                        throw new InvalidOperationException("The set of nested types cannot be changed once it's been set.");
                    }
                #pragma warning restore 420
                // ReSharper restore PossibleUnintendedReferenceComparison

            }
        }

        public string Name
        {
            get
            {
                if (m_name == null) {
                    var name = m_peFile.ReadSystemString(m_pRow->GetTypeNameIndex(m_peFile));
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_name, name, null);
                    #pragma warning restore 420
                }
                return m_name;
            }
        }
    }
}
