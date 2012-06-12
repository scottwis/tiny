// Assembly.cs
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
using System.Configuration.Assemblies;
using System.IO;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    //# Represents a managed assembly.
    public sealed unsafe class Assembly : IDisposable
    {
        PEFile m_peFile;
        ModuleCollection m_modules;
        AssemblyRow* m_assemblyRow;
        string m_name;
        IReadOnlyList<byte> m_publicKey;
        string m_culture;

        public Assembly(string fileName)
        {
            try {
                m_peFile = new PEFile(fileName);

                var assemblyRowCount = MetadataTable.Assembly.RowCount(m_peFile);
                if (assemblyRowCount == 0) {
                    throw new FileLoadException("Not an assembly", fileName);
                }
                if (assemblyRowCount > 1) {
                    throw new FileLoadException("Too many rows in the assembly table.");
                }
                
                m_modules = new ModuleCollection(m_peFile);
            }
            catch {
                Dispose();
                throw;
            }
            
        }

        public IReadOnlyList<Module> Modules
        {
            get { 
                CheckDisposed();
                return m_modules;
            }
        }

        private void CheckDisposed()
        {
            if (m_peFile == null || m_peFile.IsDisposed) {
                throw new ObjectDisposedException("Assembly");
            }
        }

        public AssemblyHashAlgorithm  HashAlgorithm
        {
            get
            {
                CheckDisposed();
                return m_assemblyRow->HashAlgorithm;
            }
        }

        public int MajorVersion
        {
            get
            {
                CheckDisposed();
                return m_assemblyRow->MajorVersion;
            }
        }

        public int MinorVersion
        {
            get
            {
                CheckDisposed();
                return m_assemblyRow->MinorVersion;
            }
        }

        public int BuildNumber
        {
            get
            {
                CheckDisposed();
                return m_assemblyRow->BuildNumber;
            }
        }

        public int RevisionNumber
        {
            get
            {
                CheckDisposed();
                return m_assemblyRow->RevisionNumber;
            }
        }

        public AssemblyFlags Flags
        {
            get
            {
                CheckDisposed();
                return m_assemblyRow->Flags;
            }
        }

        public IReadOnlyList<byte> PublicKey
        {
            get
            {
                CheckDisposed();
                if (m_publicKey == null) {
                    uint index = m_assemblyRow->GetPublicKeyOffset(m_peFile);
                    m_publicKey= m_peFile.ReadBlob(index);
                }
                return m_publicKey;
            }
        }

        public string Name
        {
            get
            {
                CheckDisposed();
                if (m_name == null) {
                    var index = m_assemblyRow->GetNameOffset(m_peFile);
                    m_name = m_peFile.ReadSystemString(index);
                }
                return m_name;
            }
        }

        public string Culture
        {
            get
            {
                CheckDisposed();
                if (m_culture == null) {
                    var index = m_assemblyRow->GetCultureOffset(m_peFile);
                    m_culture = m_peFile.ReadSystemString(index);
                }
                return m_culture;
            }
        }

        public void Dispose()
        {
            if (m_peFile != null) {
                m_peFile.Dispose();
            }

            if (m_modules != null) {
                m_modules.Dispose();
            }

            m_modules = null;
            m_peFile = null;
            m_assemblyRow = null;
        }
    }
}
