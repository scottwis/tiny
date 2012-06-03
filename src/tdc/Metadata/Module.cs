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
using Tiny.Decompiler.Metadata.Layout;

namespace Tiny.Decompiler.Metadata
{
    sealed unsafe class Module
    {
        readonly ModuleRow* m_pModuleRow;
        readonly PEFile m_peFile;
        string m_name;
        readonly bool m_containsMetadata;
        Guid? m_guid;
        readonly LiftedCollection<TypeDefinition> m_types;

        private Module(ModuleRow* moduleRow, PEFile peFile)
        {
            m_pModuleRow = moduleRow;
            m_peFile = peFile;
            m_containsMetadata = true;
            m_types = new LiftedCollection<TypeDefinition>(
                m_peFile.GetRowCount(MetadataTable.TypeDef),
                (index)=>peFile.GetTypeDefRow(index),
                (pRow)=>new TypeDefinition((TypeDefRow *)pRow, m_peFile),
                ()=>m_peFile.IsDisposed
            );
        }

        private Module(string name)
        {
            m_name = name;
            m_containsMetadata = false;
        }

        public static Module CreateNonMetadataModule(string name)
        {
            return new Module(name.CheckNotNull("name"));
        }

        public static Module CreateMetadataModule(ModuleRow* moduleRow, PEFile peFile)
        {
            Util.CheckNotNull((void *)moduleRow, "moduleRow");
            return new Module(moduleRow, peFile);
        }

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

        public IReadOnlyCollection<TypeDefinition> Types
        {
            get
            {
                CheckDisposed();
                return m_types;
            }
        }
    }
}
