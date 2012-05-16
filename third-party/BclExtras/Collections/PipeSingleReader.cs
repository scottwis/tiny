using System;
using System.Collections.Generic;
using System.Text;
using BclExtras.Threading;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Collections
{
    /// <summary>
    /// Pipe of data which is read from a single source and written by any number of consumers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ThreadSafe]
    public class PipeSingleReader<T> : IDisposable
    {
        private readonly ThreadAffinity m_affinity = new ThreadAffinity();
        private readonly Queue<T> m_queue = new Queue<T>();
        private readonly AutoResetEvent m_event = new AutoResetEvent(false);
        private readonly object m_lock = new object();
        private bool m_inputClosed;

        public PipeSingleReader()
        {
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PipeSingleReader()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_event.Close();
            }
        }

        #endregion

        public void WaitForOutput()
        {
            m_affinity.Check();
            do
            {
                lock (m_lock)
                {
                    if (m_queue.Count > 0)
                    {
                        return;
                    }
                }
                m_event.WaitOne();
            } while (true);
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Non-trivial operation")]
        public T GetNextOutput()
        {
            m_affinity.Check();
            lock (m_lock)
            {
                if (m_queue.Count > 0)
                {
                    return m_queue.Dequeue();
                }
            }

            T data;
            do
            {
                m_event.WaitOne();
            } while (!TryGetOutput(out data));

            return data;
        }

        public bool TryGetOutput(out T value)
        {
            m_affinity.Check();
            lock (m_lock)
            {
                if (m_queue.Count == 0)
                {
                    value = default(T);
                    return false;
                }

                value = m_queue.Dequeue();
                return true;
            }
        }

        public void AddInput(T value)
        {
            lock (m_lock)
            {
                if (m_inputClosed)
                {
                    throw new InvalidOperationException("Input end of channel is closed");
                }

                m_queue.Enqueue(value);
            }

            m_event.Set();
        }

        public void CloseInput()
        {
            lock (m_lock)
            {
                m_inputClosed = true;
            }
        }

    }

}
