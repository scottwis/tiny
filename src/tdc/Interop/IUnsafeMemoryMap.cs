using System;

namespace Tiny.Decompiler.Interop
{
    unsafe interface IUnsafeMemoryMap : IDisposable
    {
        void* Data { get;  }
        uint Size { get;}
    }
}
