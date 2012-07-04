// GenericParameter.cs
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
    //# Represents a type parameter of a generic MethodDefiniition or a generic TypeDefinition.
    public sealed unsafe class GenericParameter : Type
    {
        readonly GenericParameterRow* m_pRow;
        readonly PEFile m_peFile;
        readonly IGenericParameterScope m_parent;
        volatile IReadOnlyList<Type> m_constraints;
        volatile String m_name;

        internal GenericParameter(GenericParameterRow * pRow, PEFile peFile, IGenericParameterScope parent) : 
            base(TypeKind.GenericParameter)
        {
            m_pRow = (GenericParameterRow *)FluentAsserts.CheckNotNull((void*) pRow, "pRow");
            m_peFile = peFile.CheckNotNull("peFile");
            m_parent = parent.CheckNotNull("parent");
        }

        public IGenericParameterScope Parent
        {
            get 
            {
                CheckDisposed();
                return m_parent;
            }
        }

        public int Ordinal
        {
            get
            {
                CheckDisposed();
                //CONSIDER: Will this do an unaligned access? If so, we should cache the result.
                return m_pRow->Ordinal;
            }
        }

        public string Name
        {
            get
            {
                CheckDisposed();
                if (m_name == null) {
                    var name = m_peFile.ReadSystemString(m_pRow->GetNameOffset(m_peFile));
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_name, name, null);
                    #pragma warning restore 420
                }
                return m_name;
            }
        }

        public bool IsCovariant
        {
            get { return CheckFlag(GenericParameterAttributes.Covariant); }
        }

        bool CheckFlag(GenericParameterAttributes flag)
        {
            CheckDisposed();
            return (m_pRow->Flags & flag) != 0;
        }

        public bool IsContravariant
        {
            get { return CheckFlag(GenericParameterAttributes.Contravariant); }
        }

        public bool IsInvariant
        {
            get { return !(IsContravariant || IsCovariant); }
        }

        public bool HasRefrenceTypeConstraint
        {
            get { return CheckFlag(GenericParameterAttributes.ReferenceTypeConstraint); }
        }

        public bool HasValueTypeConstraint
        {
            get { return CheckFlag(GenericParameterAttributes.NotNullableValueTypeConstraint); }
        }

        public bool HasDefaultConstructorConstraint
        {
            get { return CheckFlag(GenericParameterAttributes.DefaultConstructorConstraint); }
        }

        public IReadOnlyList<Type> Constraints
        {
            get
            {
                LoadConstraints();
                return m_constraints;
            }
        }

        void LoadConstraints()
        {
            if (m_constraints == null) {
                var constraints = m_peFile.LoadIndirectChildren(
                    (uint)MetadataTable.GenericParam.RowIndex(m_pRow, m_peFile),
                    MetadataTable.GenericParamConstraint,
                    pRow=>((GenericParameterConstraintRow *)pRow)->GetOwner(m_peFile) - 1,
                    pRow=>new TypeReference(
                        ((GenericParameterConstraintRow *)pRow)->GetConstraint(m_peFile),
                        Parent.Module
                    )
                );

                #pragma warning disable 420
                Interlocked.CompareExchange(ref m_constraints, constraints, null);
                #pragma warning restore 420
            }
        }

        internal override void GetFullName(StringBuilder b)
        {
            var x = Parent as IMemberDefinitionInternal;
            if (x != null) {
                x.GetFullName(b);
            }
            else {
                b.Append(Parent.FullName);
            }
            if (Parent is TypeDefinition) {
                b.Append("!");
            }
            else {
                b.Append("!!");
            }
            b.Append("Name");
        }

        private void CheckDisposed()
        {
            if (m_peFile.IsDisposed) {
                throw new ObjectDisposedException("GenericParameter");
            }
        }
    }
}
