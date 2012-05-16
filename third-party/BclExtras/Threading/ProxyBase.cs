using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BclExtras.Threading
{

    public abstract class ProxyBase<T>
        where T : class
    {
        private List<Sink<T>> m_sinkList = new List<Sink<T>>();
        private object m_lock = new object();

        public void Subscribe(T sink, ISynchronizeInvoke invoke)
        {
            lock (m_lock)
            {
                m_sinkList.Add(new Sink<T>(sink, invoke));
            }
        }

        public void Unsubscribe(T sink)
        {
            lock (m_lock)
            {
                m_sinkList.RemoveAll(delegate(Sink<T> cur) { return Object.ReferenceEquals(sink, cur.ISink); });
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1030")] // It's specifically here to raise events
        protected void RaiseSinkCallback(SinkCallback<T> callback)
        {
            // Get the list of sinks in the lock
            List<Sink<T>> list = new List<Sink<T>>();
            lock (m_lock)
            {
                list = new List<Sink<T>>(m_sinkList);
            }

            foreach (Sink<T> current in list)
            {
                current.BeginInvoke(callback);
            }
        }
    }

}
