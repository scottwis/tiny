using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Threading
{
    /// <summary>
    /// Less than optimal way to create an IAsyncResult implementation for 
    /// a Future
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001")]
    internal class ActiveObjectAsyncResult : IAsyncResult
    {
        private ManualResetEvent m_event;
        private Future<object> m_future;

        internal Future<object> Future
        {
            get { return m_future; }
        }

        internal ActiveObjectAsyncResult(Future<object> future)
        {
            m_future = future;
        }

        /// <summary>
        /// This is called by the creator of the FutureAsyncResult instance to destroy
        /// the wait handle once the operation is completed
        /// </summary>
        internal void DestroyWaitHandle()
        {
            if (m_event != null)
            {
                m_event.Close();
            }
        }

        #region IAsyncResult Members

        object IAsyncResult.AsyncState
        {
            get { return m_future; }
        }

        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get
            {
                if (m_event == null)
                {
                    m_event = m_future.CreateWaitHandle();
                }

                return m_event;
            }
        }

        bool IAsyncResult.CompletedSynchronously
        {
            get { return false; }
        }

        bool IAsyncResult.IsCompleted
        {
            get { return m_future.HasCompleted; }
        }

        #endregion
    }
}
