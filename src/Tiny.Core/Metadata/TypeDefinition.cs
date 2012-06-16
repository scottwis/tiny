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
using System.Text;
using System.Threading;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    //# Represents the definition of a class, struct, interface, or enum type defined in a managed executable.
    public sealed unsafe class TypeDefinition : 
        Type, ITypeContainer, IMutableTypeContainer, IGenericParameterScope, IMemberDefinitionInternal
    {
        volatile bool m_didSetParent;
        volatile TypeDefinition m_parent;
        readonly TypeDefRow* m_pRow;
        volatile IReadOnlyList<TypeDefinition> m_nestedTypes;
        volatile string m_name;
        volatile string m_namespace;
        readonly Module m_module;

        internal TypeDefinition(TypeDefRow * pRow, Module module) : base(TypeKind.TypeDefinition)
        {
            m_module = module.CheckNotNull("module");
            FluidAsserts.CheckNotNull((void *)pRow, "pRow");
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
        }

        //# The set of fields defined on the type. If the class defines no fields, this will be an empty list.
        public IReadOnlyList<FieldDefinition> Fields
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        //# The set of methods defined on the type, or null if the type defines no methods.
        public IReadOnlyList<MethodDefinition> Methods
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        //# The type this type inherits from, or null if the type has no base class.
        public Type BaseType
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        //# The set of interfaces implemented by the type. If the type implements no interfaces this will be an empty
        //# list.
        public IReadOnlyList<Type> ImplementedInterfaces
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        //# The set of events defined by the type.
        public IReadOnlyList<Event> Events
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        //# The set of properties defined by the type.
        public IReadOnlyList<Property> Properties
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        //# The set of generic parameters declared by the type.
        public IReadOnlyList<GenericParameter> GenericParameters
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        //# The set of custom attributes applied to the type.
        public IReadOnlyList<CustomAttribute> CustomAttributes
        {
            get
            {
                //TODO: Implement this
                throw new NotImplementedException();
            }
        }

        public Module Module
        {
            get { return m_module; }
        }

        void CheckDisposed()
        {
            if (Module == null || Module.PEFile.IsDisposed) {
                throw new ObjectDisposedException("TypeDefinition");
            }
        }
    }
}
