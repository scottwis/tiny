using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Tiny.Decompiler.Metadata
{
    //# Represents the collection of modules in an assembly.
    sealed class ModuleCollection : IReadOnlyList<Module>, IDisposable
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Module> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public Module this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
