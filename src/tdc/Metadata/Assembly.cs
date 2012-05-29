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
using System.Threading;

namespace Tiny.Decompiler.Metadata
{
    //# Represents a managed assembly.
    sealed unsafe class Assembly : IDisposable
    {
        PEFile m_peFile;
        ModuleCollection m_modules;

        public Assembly(string fileName)
        {
            try {
                m_peFile = new PEFile(fileName);
                if (m_peFile.GetRowCount(MetadataTable.Assembly) == 0) {
                    throw new FileLoadException("Not an assembly", fileName);
                }
                m_modules = new ModuleCollection(m_peFile);
            }
            catch {
                Dispose();
                throw;
            }
            
        }

        public ModuleCollection Modules
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
                #error "Implement this"
            }
        }

        public int MajorVersion
        {
            get
            {
                CheckDisposed();
                #error "Implement this"
            }
        }

        public int MinorVersion
        {
            get
            {
                CheckDisposed();
                #error "Implement this"
            }
        }

        public int BuildNumber
        {
            get
            {
                CheckDisposed();
                #error "Implement this"
            }
        }

        public int RevisionNumber
        {
            get
            {
                CheckDisposed();
                #error "Implement this"
            }
        }

        public AssemblyFlags Flags
        {
            get
            {
                CheckDisposed();
                #error "Implement this"
            }
        }

        public IReadOnlyList<byte> PublicKey
        {
            get
            {
                CheckDisposed();
                #error "Implement this"
            }
        }

        public string Name
        {
            get
            {
                CheckDisposed();
                #error "Implement this"
            }
        }

        public string Culture
        {
            get
            {
                CheckDisposed();
                #error "Implement this"
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
        }
    }
}
