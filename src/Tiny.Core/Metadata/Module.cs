// Module.cs
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
using System.Linq;
using System.Threading;
using Tiny.Collections;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    //# Represents a module in a managed assembly.
    public sealed unsafe class Module : ITypeContainer, IMutableTypeContainer
    {
        readonly Object m_lockObject;
        readonly ModuleRow* m_pModuleRow;
        readonly PEFile m_peFile;
        string m_name;
        readonly bool m_containsMetadata;
        Guid? m_guid;
        volatile IReadOnlyList<TypeDefinition> m_types;

        private Module(ModuleRow* moduleRow, PEFile peFile)
        {
            m_lockObject = new object();
            m_pModuleRow = moduleRow;
            m_peFile = peFile;
            m_containsMetadata = true;
        }

        private Module(string name)
        {
            m_name = name;
            m_containsMetadata = false;
        }

        internal static Module CreateNonMetadataModule(string name)
        {
            return new Module(name.CheckNotNull("name"));
        }

        internal static Module CreateMetadataModule(ModuleRow* moduleRow, PEFile peFile)
        {
            FluidAsserts.CheckNotNull((void *)moduleRow, "moduleRow");
            return new Module(moduleRow, peFile);
        }

        //# Indicates wether or not the module contains meta-data. We model
        //# non metadata (i.e. resource) entries in the File table as modules.
        //# throws: [ObjectDisposedException] if the defining assembly has been disposed.
        public bool ContainsMetadata
        {
            get
            {
                CheckDisposed();
                return m_containsMetadata;
            }
        }

        void CheckDisposed()
        {
            if (m_containsMetadata && m_peFile.IsDisposed) {
                throw new ObjectDisposedException("Module");
            }
        }

        //# The name of the module.
        //# throws: [ObjectDisposedException] if the defining assembly has been disposed.
        public string Name
        {
            get
            {
                CheckDisposed();
                if (m_name == null && m_containsMetadata) {
                    m_name = m_peFile.ReadSystemString(m_pModuleRow->GetNameOffset(m_peFile));
                }
                return m_name;
            }
        }

        //# The "module version id" guid. A guid that uniquely the current version of the module.
        public Guid Guid
        {
            get
            { 
                CheckDisposed();
                if (!m_containsMetadata) {
                    throw new InvalidOperationException("Non meta-data modules do not define a guid.");
                }
                if (m_guid == null) {
                    m_guid = m_peFile.ReadGuid(m_pModuleRow->GetMvidOffset(m_peFile));
                }
                return m_guid.Value;
            }
        }

        private void LoadTypes()
        {
            var allTypes = new LiftedList<TypeDefinition>(
                MetadataTable.TypeDef.RowCount(m_peFile),
                (index) => m_peFile.GetRow(index, MetadataTable.TypeDef),
                (pRow) => new TypeDefinition((TypeDefRow*)pRow, m_peFile),
                () => m_peFile.IsDisposed
            );

            var nestedTypes = new LiftedValueTypeList<NestedTypeInfo>(
                MetadataTable.NestedClass.RowCount(m_peFile),
                index=>m_peFile.GetRow(index, MetadataTable.NestedClass),
                x=> {
                    var pRow = (NestedClassRow*) x;
                    return new NestedTypeInfo(
                        allTypes[(int)(pRow->GetNestedClass(m_peFile) - 1)], 
                        allTypes[(int)(pRow->GetEnclosingClass(m_peFile) - 1)]
                    );
                },
                ()=>m_peFile.IsDisposed
            );

            var enclosingTypes = nestedTypes.Select(x => x.NestedType.DeclaringType = x.EnclosingType).ToHashSet();
            allTypes.GroupBy(x => x.DeclaringType).ForEach(
                x => (x.Key ?? (IMutableTypeContainer)this).Types = x.ToList().AsReadOnly()
            );
            allTypes.Where(
                x => !enclosingTypes.Contains(x)
            ).ForEach(
                (IMutableTypeContainer x) => x.Types = new List<TypeDefinition>().AsReadOnly()
            );
        }
        
        public IReadOnlyList<TypeDefinition> Types
        {
            get
            {
                CheckDisposed();
                if (m_types == null) {
                    lock(m_lockObject) {
                        if (m_types == null) {
                            LoadTypes();
                        }
                    }
                }
                return m_types;
            }
        }

        IReadOnlyList<TypeDefinition> IMutableTypeContainer.Types
        {
            set
            {
                CheckDisposed();
                #pragma warning disable 420
                // ReSharper disable PossibleUnintendedReferenceComparison
                    Interlocked.CompareExchange(ref m_types, value, null);
                    if (m_types != value) {
                        throw new InvalidOperationException("The types collection cannot be changed once it has been set.");
                    }
                // ReSharper restore PossibleUnintendedReferenceComparison
                #pragma warning restore 420
            }
        }
    }
}
