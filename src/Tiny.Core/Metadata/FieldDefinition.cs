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
    public unsafe sealed class FieldDefinition : IMemberDefinitionInternal
    {
        volatile string m_name;
        readonly FieldRow * m_pRow;
        readonly TypeDefinition m_declaringType;
        volatile Type m_fieldType;
        volatile IReadOnlyList<CustomAttribute> m_customAttributes;
        volatile MarshalInfo m_marshalInfo;
        volatile bool m_didSetMarshalInfo;

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
                ((IMemberDefinitionInternal) this).GetFullName(b);
                return b.ToString();
            }
        }

        void IMemberDefinitionInternal.GetFullName(StringBuilder b)
        {
            DeclaringType.GetFullName(b);
            b.AppendFormat(".{0}", Name);
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
                if (m_customAttributes == null)
                {
                    var attributes = m_declaringType.Module.PEFile.LoadCustomAttributes(MetadataTable.Field, m_pRow);
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_customAttributes, attributes, null);
                    #pragma warning restore 420
                }
                return m_customAttributes;
            }
        }

        public bool IsPublic
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.FieldAccessMask) == FieldAttributes.Public;
            }
        }

        public bool IsPrivate
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.FieldAccessMask) == FieldAttributes.Private;
            }
        }

        public bool IsProtected
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.FieldAccessMask) == FieldAttributes.Family;
            }
        }

        public bool IsInternalAndProtected
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.FieldAccessMask) == FieldAttributes.FamAndAssembly;
            }
        }

        public bool IsInternalOrProtected
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.FieldAccessMask) == FieldAttributes.FamOrAssembly;
            }
        }

        public bool IsInternal
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.FieldAccessMask) == FieldAttributes.Assembly;
            }
        }

        public bool HasSpecialName
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.SpecialName) != 0;
            }
        }

        public bool HasRuntimeSpecialName
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.RTSpecialName) != 0;
            }
        }

        public bool IsCompilerControlled
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.FieldAccessMask) == FieldAttributes.CompilerControlled;
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
                if (! m_didSetMarshalInfo) {
                    var peFile = DeclaringType.Module.PEFile;
                    var index = MetadataTable.FieldMarshal.Find(
                        new HasFieldMarshal(
                            MetadataTable.Field,
                            MetadataTable.Field.RowIndex(m_pRow, peFile)
                        ),
                        pRow => ((FieldMarshalRow*)pRow)->GetParent(peFile),
                        peFile
                    );

                    if (index >= 0) {
                        var marshalInfo = SignatureParser.ParseMarshalDescriptor(peFile.ReadBlob(
                            ((FieldMarshalRow*) MetadataTable.FieldMarshal.GetRow(index, peFile))->GetNativeTypeOffset(
                                peFile
                            )
                        ));

                        #pragma warning disable 420
                        Interlocked.CompareExchange(ref m_marshalInfo, marshalInfo, null);
                        #pragma warning restore 420
                    }
                    m_didSetMarshalInfo = true;
                }
                return m_marshalInfo;
            }
        }

        public bool IsStatic
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.Static) != 0;
            }
        }

        public bool IsConst
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.Literal) != 0;
            }
        }

        public bool IsReadonly
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & FieldAttributes.InitOnly) != 0;
            }
        }

        public uint? RVA
        {
            get
            {
                CheckDisposed();
                if ((m_pRow->Flags & FieldAttributes.HasFieldRVA) != 0) {
                    return m_declaringType.Module.GetRVA(m_pRow);
                }
                return null;
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
