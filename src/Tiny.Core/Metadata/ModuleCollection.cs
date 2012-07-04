// ModuleCollection.cs
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
using System.Collections;
using System.Collections.Generic;
using Tiny.Collections;
using Tiny.Metadata.Layout;

namespace Tiny.Metadata
{
    //# Represents the collection of modules in an assembly.
    sealed class ModuleCollection : ReadonlyListBase<Module>, IDisposable
    {
        //# Used to syncronize loading of the PE files for the non-manifest modules.
        object m_lockObject;

        //# The PE file containing the manifest module of the assembly. 
        PEFile m_mainFile;

        //# The list of other modules (besides the main module) in an asssembly. The array is loaded lazily. Each
        //# element will either be a PEFile (for meta-data modules) or a Module (for non meta-data modules).
        object[] m_otherModules;
        Assembly m_assembly;

        internal ModuleCollection(Assembly assembly, PEFile mainFile)
        {
            try {
                m_assembly = assembly.CheckNotNull("assembly");
                m_mainFile = mainFile.CheckNotNull("mainFile");
                m_otherModules = new object[MetadataTable.File.RowCount(m_mainFile)];
                m_lockObject = new object();
            }
            catch {
                Dispose();
                throw;
            }
        }

        public override IEnumerator<Module> GetEnumerator()
        {
            CheckDisposed();
            for (var i = 0; i < Count; ++i) {
                yield return this[i];
            }
        }

        protected override void CheckDisposed()
        {
            if (m_mainFile == null || m_mainFile.IsDisposed) {
                throw new ObjectDisposedException("ModuleCollection");
            }
        }

        public override int Count
        {
            get
            {
                CheckDisposed();
                return m_otherModules.Length + 1;
            }
        }

        public override Module this[int index]
        {
            get
            {
                CheckDisposed();
                if (index < 0 || index > m_otherModules.Length) {
                    throw new IndexOutOfRangeException();
                }
                if (index == 0) {
                    return m_mainFile.Module;
                }
                LoadModule((index - 1));
                var ret = m_otherModules[index - 1];
                var peFile = ret as PEFile;
                if (peFile != null) {
                    return peFile.Module;
                }
                return ret.AssumeIs<Module>();
            }
        }

        unsafe void LoadModule(int index)
        {
            if (m_otherModules[index] == null) {
                lock (m_lockObject) {
                    if (m_otherModules[index] == null) {
                        var f = (FileRow *)m_mainFile.GetRow(new ZeroBasedIndex(index),MetadataTable.File);
                        if ((f->Flags & FileAttributes.ContainsNoMetadata) != 0) {
                            m_otherModules[index] = Module.CreateNonMetadataModule(
                                m_assembly,
                                m_mainFile.ReadSystemString(
                                    f->GetNameOffset(m_mainFile)
                                )
                            );
                        }
                        else {
                            PEFile peFile = null;
                            try {
                                peFile = new PEFile(m_assembly, m_mainFile.ReadSystemString(f->GetNameOffset(m_mainFile)));
                                m_otherModules[index] = peFile;
                            }
                            catch {
                                if (peFile != null) {
                                    peFile.Dispose();
                                }
                                throw;
                            }
                        }
                    }
                }
            }
        }

        //# Cleans up native resources used by the module collection.
        //# remarks: This will dispose resources for all contained modules, including the manifest module.
        //# It will invalidate the containing assembly and all objects it contains.
        public void Dispose()
        {
            if (m_mainFile != null) {
                m_mainFile.Dispose();
            }

            if (m_otherModules != null) {
                foreach (var obj in m_otherModules) {
                    var peFile = obj as PEFile;
                    if (peFile != null) {
                        peFile.Dispose();
                    }
                }
            }
            m_otherModules = null;
            m_mainFile = null;
            m_assembly = null;
        }
    }
}
