// MethodDefinition.cs
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Tiny.Collections;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    public sealed unsafe class MethodDefinition : Method, IGenericParameterScope, IMemberDefinitionInternal
    {
        readonly TypeDefinition m_declaringType;
        readonly MethodDefRow* m_pRow;
        volatile string m_name;
        volatile IReadOnlyList<CustomAttribute> m_customAttributes;
        volatile IReadOnlyList<GenericParameter> m_genericParameters;
        volatile Method m_signature;
        volatile IReadOnlyList<Parameter> m_parameters;
        volatile Parameter m_returnTypeParameter;

        internal MethodDefinition(MethodDefRow * pRow, TypeDefinition declaringType)
        {
            m_pRow = (MethodDefRow*)FluentAsserts.CheckNotNull((void *)pRow, "pRow");
            m_declaringType = declaringType.CheckNotNull("declaringType");
        }

        public override string Name
        {
            get
            {
                CheckDisposed();
                if (m_name == null) {
                    var peFile = DeclaringType.Module.PEFile;
                    var name = peFile.ReadSystemString(m_pRow->GetNameOffset(peFile));
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
                var builder = new StringBuilder();
                ((IMemberDefinitionInternal)this).GetFullName(builder);
                return builder.ToString();
            }
        }

        void IMemberDefinitionInternal.GetFullName(StringBuilder builder)
        {
            DeclaringType.GetFullName(builder);
            builder.AppendFormat(".{0}", Name);
            PrintGenericParameters(builder);
            PrintParameters(builder);
        }

        void CheckDisposed()
        {
            if (Module.PEFile.IsDisposed) {
                throw new ObjectDisposedException("MethodDefinition");
            }
        }

        void PrintParameters(StringBuilder builder)
        {
            Parameters.Print(
                builder, 
                ",", 
                "(", 
                ")",
                (p, b)=> {
                    p.ParameterType.GetFullName(b);
                    b.AppendFormat(" {0}", p.Name);
                }
            );
        }

        void PrintGenericParameters(StringBuilder builder)
        {
            if (GenericParameterCount != 0) {
                GenericParameters.Print(builder, ",", "<", ">", (gp, b) => b.Append(gp.Name));
            }
        }

        public TypeDefinition DeclaringType
        {
            get
            {
                return m_declaringType;
            }
        }

        public IReadOnlyList<CustomAttribute> CustomAttributes
        {
            get
            {
                CheckDisposed();
                if (m_customAttributes == null) {
                    var attributes = Module.PEFile.LoadCustomAttributes(MetadataTable.MethodDef, m_pRow);
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
                return (m_pRow->Flags & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
            }
        }

        public bool IsPrivate
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
            }
        }

        public bool IsProtected
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.MemberAccessMask) == MethodAttributes.Family;
            }
        }

        public bool IsInternal
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly;
            }
        }

        public bool IsInternalOrProtected
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.MemberAccessMask) == MethodAttributes.FamilyOrAssembly;
            }
        }

        public bool IsInternalAndProtected
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.MemberAccessMask) == MethodAttributes.FamilyAndAssembly;
            }
        }

        public bool IsAbstract
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.Abstract) != 0;
            }
        }

        public bool IsSealed
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.Final) != 0;
            }
        }

        public bool HasSpecialName
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.SpecialName) != 0;
            }
        }

        public bool HasRuntimeSpecialName
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.RTSpecialName) != 0;
            }
        }

        public bool IsCompilerControlled
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.MemberAccessMask) == MethodAttributes.CompilerControlled;
            }
        }

        public IReadOnlyList<GenericParameter> GenericParameters
        {
            get
            {
                CheckDisposed();
                if (m_genericParameters == null) {
                    var rows = Module.GetGenericParameterRows(
                        new TypeOrMethodDef(
                            MetadataTable.MethodDef,
                            MetadataTable.MethodDef.RowIndex(m_pRow, Module.PEFile)
                        )
                    );

                    var genericParameters = new LiftedList<GenericParameter>(
                        rows.Count,
                        index => (void*) rows[index],
                        pRow => new GenericParameter((GenericParameterRow*) pRow, Module.PEFile, this),
                        () => Module.PEFile.IsDisposed
                    );

                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_genericParameters, genericParameters, null);
                    #pragma warning restore 420
                }
                return m_genericParameters;
            }
        }

        public Module Module
        {
            get
            {
                CheckDisposed();
                return DeclaringType.Module;
            }
        }

        private void LoadSignature()
        {
            if (m_signature == null) {
                var peFile = DeclaringType.Module.PEFile;
                var bytes = peFile.ReadBlob(m_pRow->GetSignatureOffset(peFile));
                var signature = SignatureParser.ParseMethodSignature(bytes, this);
                #pragma warning disable 420
                Interlocked.CompareExchange(ref m_signature, signature, null);
                #pragma warning restore 420
            }
        }

        public override Type ReturnType
        {
            get
            {
                CheckDisposed();
                LoadSignature();
                return m_signature.ReturnType;
            }
        }

        public override IReadOnlyList<Parameter> Parameters
        {
            get
            {
                CheckDisposed();
                LoadSignature();

                if (m_parameters == null) {
                    var peFile = DeclaringType.Module.PEFile;
                    var firstMemberIndex = (ZeroBasedIndex) m_pRow->GetParamListIndex(peFile);
                    ZeroBasedIndex lastMemberIndex;
                    var tableIndex = MetadataTable.MethodDef.RowIndex(m_pRow, peFile);
                    if (tableIndex == MetadataTable.MethodDef.RowCount(peFile) - 1) {
                        lastMemberIndex = new ZeroBasedIndex(MetadataTable.Param.RowCount(peFile));
                    }
                    else {
                        lastMemberIndex = (ZeroBasedIndex) (((MethodDefRow*) MetadataTable.MethodDef.GetRow(
                            tableIndex + 1,
                            peFile
                        ))->GetParamListIndex(peFile));
                    }

                    var hasReturnTypeParameter = false;

                    if ((lastMemberIndex - firstMemberIndex) > 0) {
                        var pFirstParam = (ParamRow*) MetadataTable.Param.GetRow(firstMemberIndex, peFile);
                        hasReturnTypeParameter = pFirstParam->Sequence == 0;
                    }

                    IReadOnlyList<Parameter> parameters = new LiftedList<Parameter>(
                        (lastMemberIndex - firstMemberIndex).Value,
                        index => MetadataTable.Param.GetRow(firstMemberIndex + index, peFile),
                        (pRow, index) => {
                            if (index == 0 && hasReturnTypeParameter) {
                                return new Parameter((ParamRow*) pRow, m_signature.ReturnType);
                            }
                            if (hasReturnTypeParameter) {
                                return new Parameter((ParamRow*) pRow, m_signature.Parameters[index - 1]);
                            }
                            return new Parameter((ParamRow*) pRow, m_signature.Parameters[index]);
                        },
                        () => peFile.IsDisposed
                    );

                    if (hasReturnTypeParameter) {
                        #pragma warning disable 420
                        Interlocked.CompareExchange(ref m_returnTypeParameter, parameters[0], null);
                        #pragma warning restore 420
                        parameters = parameters.SubList(1);
                    }

                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_parameters, parameters, null);
                    #pragma warning restore 420
                }

                return m_parameters;
            }
            
        }

        public override bool HasThis
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public override bool ExplicitThis
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public override CallingConvention CallingConvention
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public override int GenericParameterCount
        {
            get
            {
                CheckDisposed();
                return GenericParameters.Count;
            }
        }

        public IReadOnlyList<Method> ImplementedMethods
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public IReadOnlyList<Instruction> Body
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool HasBody
        {
            get
            {
                CheckDisposed();
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public bool IsNewSlot
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.VtableLayoutMask) == MethodAttributes.NewSlot;
            }
        }

        public bool IsVirtual
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.Virtual) != 0;
            }
        }

        public bool IsHideByName
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.HideBySig) == 0;
            }
        }

        public bool IsPinvoke
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.PInvokeImpl) != 0;
            }
        }

        public bool HasDeclarativeSecurity
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.HasSecurity) != 0;
            }
        }

        public bool RequiresSecureObject
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.RequireSecObject) != 0;
            }
        }

        public bool IsStatic
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & MethodAttributes.Static) != 0;
            }
        }

        public MethodImplOptions ImplementationOptions
        {
            get { return (MethodImplOptions)(m_pRow->ImplFlags & ~(MethodImplAttributes.CodeTypeMask)); }
        }

        public MethodCodeType CodeType
        {
            get { return (MethodCodeType) (m_pRow->ImplFlags & (MethodImplAttributes.CodeTypeMask)); }
        }

        //# If the method is an event accessor, returns the event it is associated with,
        //# and null otherwise.
        public Event AssociatedEvent
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        //# If the method is a property accessor, returns the property it is associated with,
        //# and null otherwise.
        public Property AssociatedProperty
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }
    }
}
