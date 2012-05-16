using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BclExtras.Threading
{
    [Immutable]
    public sealed class ThreadAffinity
    {
        private readonly int m_threadId;

        public ThreadAffinity()
        {
            m_threadId = Thread.CurrentThread.ManagedThreadId;
        }

        public void Check()
        {
            if (Thread.CurrentThread.ManagedThreadId != m_threadId)
            {
                var msg = String.Format(
                    "Call to class with affinity to thread {0} detected from thread {1}.",
                    m_threadId,
                    Thread.CurrentThread.ManagedThreadId);
                throw new InvalidOperationException(msg);
            }
        }
    }
}
