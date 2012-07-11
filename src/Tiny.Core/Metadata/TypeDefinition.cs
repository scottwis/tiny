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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Tiny.Collections;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    //# Represents the definition of a class, struct, interface, or enum type defined in a managed executable.
    public sealed unsafe class TypeDefinition : 
        Type, ITypeContainer, IMutableTypeContainer, IGenericParameterScope, IMemberDefinitionInternal
    {
        volatile bool m_didSetParent;
        volatile bool m_didSetBase;
        volatile TypeDefinition m_parent;
        readonly TypeDefRow* m_pRow;
        volatile IReadOnlyList<TypeDefinition> m_nestedTypes;
        volatile string m_name;
        volatile string m_namespace;
        readonly Module m_module;
        volatile IReadOnlyList<FieldDefinition> m_fields;
        volatile IReadOnlyList<MethodDefinition> m_methods;
        volatile Type m_baseType;
        volatile IReadOnlyList<Type> m_implementedInterfaces;
        volatile IReadOnlyList<Event> m_events;
        volatile IReadOnlyList<Property> m_properties;
        volatile IReadOnlyList<GenericParameter> m_genericParameters;
        volatile IReadOnlyList<CustomAttribute> m_customAttributes;

        internal TypeDefinition(TypeDefRow * pRow, Module module) : base(TypeKind.TypeDefinition)
        {
            m_module = module.CheckNotNull("module");
            FluentAsserts.CheckNotNull((void *)pRow, "pRow");
            m_pRow = pRow;
        }

       //# Returns the type containing this type, or null if the type is not nested.
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

        //# Returns a list of types nested inside this one. This should never return a null value.
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

        //# The name of the type.
        public string Name
        {
            get
            {
                CheckDisposed();
                if (m_name == null) {
                    var name = Module.PEFile.ReadSystemString(m_pRow->GetTypeNameIndex(Module.PEFile));
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_name, name, null);
                    #pragma warning restore 420
                }
                return m_name.AssumeNotNull();
            }
        }

        //# The namespace the type is defined in. If the type is nested this will return the namespace of the containing type.
        public string Namespace
        {
            get
            {
                CheckDisposed();
                if (DeclaringType != null) {
                    return DeclaringType.Namespace;
                }
                if (m_namespace == null) {
                    var name = Module.PEFile.ReadSystemString(m_pRow->GetTypeNamespaceIndex(Module.PEFile));
                    #pragma warning disable 420
                        Interlocked.CompareExchange(ref m_namespace, name, null);
                    #pragma warning restore 420
                }
                return m_namespace.AssumeNotNull();
            }
        }

        void IMemberDefinitionInternal.GetFullName(StringBuilder b)
        {
            GetFullName(b);
        }

        internal override void GetFullName(StringBuilder b)
        {
            CheckDisposed();
            if (DeclaringType != null) {
                DeclaringType.GetFullName(b);
                b.AppendFormat("+{0}", Name);
            }
            else if (Namespace.Equals("")) {
                b.Append(Name);
            }
            else  {
                b.AppendFormat("{0}.{1}", Namespace, Name);
            }
            if (GenericParameters.Count != 0) {
                GenericParameters.Print(b, ",", "<", ">", (gp, builder)=>builder.Append(gp.Name));
            }
        }

        //TODO: Factor GetMembers into a general helper in PEFile, so that it can also be used by
        //MethodDefinition.
        LiftedList<T> LoadDirectChildren<T>(
            MetadataTable childTable,
            GetTokenDelegate tokenSelector,
            CreateObjectDelegate<T> createObject
        ) where T : class
        {
            return Module.PEFile.LoadDirectChildren(
                childTable,
                tokenSelector,
                createObject,
                MetadataTable.TypeDef,
                m_pRow
            );
        }

        IReadOnlyList<T> LoadDirectChildrenFromMap<T>(
            MetadataTable mapTable,
            MetadataTable childTable,
            UnsafeSelector<OneBasedIndex> parentSelector,
            GetTokenDelegate tokenSelector,
            CreateObjectDelegate<T> factory
        ) where T : class
        {
            //It is valid for the map table to not be sorted. However, I don't expect this to happen in
            //practice, so for now we throw an exception in that case. If we do end up needing to support
            //assemblies with unsorted meta-data tables then we will need to add some sort of fallback
            //here.
            mapTable.IsSorted(Module.PEFile).Assume("The table is not sorted");

            var mapIndex = mapTable.Find(
                (OneBasedIndex)MetadataTable.TypeDef.RowIndex(m_pRow, Module.PEFile),
                parentSelector,
                Module.PEFile
            );

            IReadOnlyList<T> ret;
            if (mapIndex < 0) {
                ret = new List<T>(0).AsReadOnly();
            }
            else {
                ret = Module.PEFile.LoadDirectChildren(
                    childTable,
                    tokenSelector,
                    factory,
                    mapTable,
                    mapTable.GetRow(mapIndex, Module.PEFile)
                );
            }
            return ret;
        }

        //# The set of fields defined on the type. If the class defines no fields, this will be an empty list.
        public IReadOnlyList<FieldDefinition> Fields
        {
            get
            {
                CheckDisposed();
                if (m_fields == null) {
                    var peFile = Module.PEFile;
                    var fields = LoadDirectChildren(
                        MetadataTable.Field,
                        (pRow)=>((TypeDefRow *)pRow)->GetFieldListToken(peFile),
                        x => new FieldDefinition((FieldRow*)x, this)
                    );
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_fields, fields, null);
                    #pragma warning restore 420
                }
                return m_fields;
            }
        }

        //# The set of methods defined on the type, or null if the type defines no methods.
        public IReadOnlyList<MethodDefinition> Methods
        {
            get
            {
                CheckDisposed();
                if (m_methods == null) {
                    var peFile = Module.PEFile;
                    var methods = LoadDirectChildren(
                        MetadataTable.MethodDef,
                        (pRow) => ((TypeDefRow*)pRow)->GetMethodListToken(peFile),
                        pRow => new MethodDefinition((MethodDefRow*)pRow, this)
                    );
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_methods, methods, null);
                    #pragma warning restore 420
                }
                return m_methods;
            }
        }

        //# The type this type inherits from, or null if the type has no base class.
        public Type BaseType
        {
            get
            {
                CheckDisposed();
                if (! m_didSetBase) {
                    var token = m_pRow->GetExtendsToken(Module.PEFile);
                    if (!token.IsNull) {
                        var baseType = new TypeReference(token, Module);
                        #pragma warning disable 420
                        Interlocked.CompareExchange(ref m_baseType, baseType, null);
                        #pragma warning restore 420
                    }
                    m_didSetBase = true;
                }
                return BaseType;
            }
        }

        //# The set of interfaces implemented by the type. If the type implements no interfaces this will be an empty
        //# list.
        public IReadOnlyList<Type> ImplementedInterfaces
        {
            get
            {
                CheckDisposed();
                if (m_implementedInterfaces == null) {
                    var implementedInterfaces = Module.PEFile.LoadIndirectChildren(
                        (OneBasedIndex)MetadataTable.TypeDef.RowIndex(m_pRow, Module.PEFile),
                        MetadataTable.InterfaceImpl,
                        pRow => ((InterfaceImplRow*)pRow)->GetClass(Module.PEFile),
                        pRow => new TypeReference(
                            ((InterfaceImplRow*)pRow)->GetInterface(Module.PEFile),
                            Module
                        )
                    );

                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_implementedInterfaces, implementedInterfaces, null);
                    #pragma warning restore 420
                }
                    
                return m_implementedInterfaces;
            }
        }

        //# The set of events defined by the type.
        public IReadOnlyList<Event> Events
        {
            get
            {
                CheckDisposed();
                if (m_events == null) {
                    var peFile = Module.PEFile;
                    var events = LoadDirectChildrenFromMap(
                        MetadataTable.EventMap, 
                        MetadataTable.Event, pRow => ((EventMapRow*) pRow)->GetParent(peFile),
                        (pRow) => ((EventMapRow*)pRow)->GetEventListIndex(peFile),
                        pRow => new Event((EventRow*) pRow, this)
                    );

                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_events, events, null);
                    #pragma warning restore 420
                }
                return m_events;
            }
        }

        //# The set of properties defined by the type.
        public IReadOnlyList<Property> Properties
        {
            get
            {
                CheckDisposed();
                if (m_properties == null) {
                    var peFile = Module.PEFile;
                    var properties = LoadDirectChildrenFromMap(
                        MetadataTable.PropertyMap,
                        MetadataTable.Property,
                        pRow => ((PropertyMapRow*) pRow)->GetParent(peFile),
                        (pRow) => ((PropertyMapRow*) pRow)->GetPropertyListIndex(peFile),
                        pRow => new Property((PropertyRow*) pRow, this)
                    );

                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_properties, properties, null);
                    #pragma warning restore 420
                }
                return m_properties;
            }
        }

        //# The set of generic parameters declared by the type.
        public IReadOnlyList<GenericParameter> GenericParameters
        {
            get
            {
                CheckDisposed();
                if (m_genericParameters == null) {
                    var rows = Module.GetGenericParameterRows(new TypeOrMethodDef(
                        MetadataTable.TypeDef,
                        MetadataTable.TypeDef.RowIndex(m_pRow, Module.PEFile)
                    ));

                    var genericParameters = new LiftedList<GenericParameter>(
                        rows.Count,
                        index => (void*)rows[index],
                        pRow => new GenericParameter((GenericParameterRow*)pRow, Module.PEFile, this),
                        () => Module.PEFile.IsDisposed
                    );

                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_genericParameters, genericParameters, null);
                    #pragma warning restore 420
                }
                return m_genericParameters;
            }
        }

        //# The set of custom attributes applied to the type.
        public IReadOnlyList<CustomAttribute> CustomAttributes
        {
            get
            {
                CheckDisposed();
                if (m_customAttributes == null) {
                    var attributes = Module.PEFile.LoadCustomAttributes(MetadataTable.TypeDef, m_pRow);
                    #pragma warning disable 420
                    Interlocked.CompareExchange(ref m_customAttributes, attributes, null);
                    #pragma warning restore 420
                }
                return m_customAttributes;
            }
        }

        public Module Module
        {
            get { return m_module; }
        }

        public bool IsInterface
        {
            get {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface;
            }
        }

        public bool IsClass
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Class;
            }
        }

        public bool IsCompilerControlled
        {
            get
            {
                CheckDisposed();
                return false;
            }
        }

        public bool IsPublic
        {
            get
            {
                CheckDisposed();
                var accessibility = (m_pRow->Flags & TypeAttributes.VisibilityMask) ;
                return accessibility == TypeAttributes.Public || accessibility == TypeAttributes.NestedPublic;
            }
        }

        public bool IsPrivate
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPrivate;
            }
        }

        public bool IsProtected
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily;
            }
        }

        public bool IsInternal
        {
            get
            {
                CheckDisposed();
                var accessibility = (m_pRow->Flags & TypeAttributes.VisibilityMask);
                return accessibility == TypeAttributes.NotPublic || accessibility == TypeAttributes.NestedFamily;
            }
        }

        public bool IsInternalOrProtected
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamORAssem;
            }
        }

        public bool IsInternalAndProtected
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamANDAssem;
            }
        }

        public LayoutKind Layout
        {
            get
            {
                CheckDisposed();
                var ret = (LayoutKind)(((int)(m_pRow->Flags & TypeAttributes.LayoutMask)) >> 7);
                return ret.AssumeDefined("Invalid Layout Kind");
            }
        }

        public bool IsAbstract
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.Abstract) != 0;
            }
        }

        public bool IsSealed
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.Sealed) != 0;
            }
        }

        public StringFormat StringFormat
        {
            get
            {
                CheckDisposed();
                return (StringFormat) (m_pRow->Flags & TypeAttributes.StringFormatMask);
            }
        }

        public bool IsBeforeFieldInit
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.BeforeFieldInit) != 0;
            }
        }

        public bool HasSpecialName
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.SpecialName) != 0;
            }
        }

        public bool HasRuntimeSpecialName
        {
            get
            {
                CheckDisposed();
                return (m_pRow->Flags & TypeAttributes.RTSpecialName) != 0;
            }
        }

        bool IMemberDefinition.IsStatic
        {
            get
            {
                CheckDisposed();
                return false;
            }
        }

        void CheckDisposed()
        {
            if (Module == null || Module.PEFile.IsDisposed) {
                throw new ObjectDisposedException("TypeDefinition");
            }
        }
    }
}
