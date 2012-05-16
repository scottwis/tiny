using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras
{
    /// <summary>
    /// Useful class for Visual Basic apps (VB does'n support destructors)
    /// </summary>
    public abstract class DisposeBase : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposeBase()
        {
            Dispose(false);
        }

        protected abstract void Dispose(bool disposing);
    }
}
