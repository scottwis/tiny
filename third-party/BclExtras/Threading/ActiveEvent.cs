using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Threading
{
    public sealed class ActiveEvent<T>
        where T : EventArgs
    {
        private EventHandler<T> m_event;
        private ActiveObject m_activeObject;

        public ActiveEvent(ActiveObject ao)
        {
            Contract.ThrowIfNull(ao);
            m_activeObject = ao;
        }

        public Future Add(EventHandler<T> handler)
        {
            Contract.ThrowIfNull(handler);
            return m_activeObject.CallOnBackground(delegate { m_event += handler; });
        }

        public Future Remove(EventHandler<T> handler)
        {
            Contract.ThrowIfNull(handler);
            return m_activeObject.CallOnBackground(delegate { m_event -= handler; });
        }

        [SuppressMessage("Microsoft.Security", "CA2109")]
        [SuppressMessage("Microsoft.Design", "CA1030")]
        public Future Raise(object sender, T args)
        {
            return m_activeObject.CallOnBackground(delegate { RaiseImmediate(sender, args); });
        }

        [SuppressMessage("Microsoft.Security", "CA2109")]
        [SuppressMessage("Microsoft.Design", "CA1030")]
        public void RaiseImmediate(object sender, T args)
        {
            m_activeObject.EnsureOnBackgroundThread();
            if (m_event != null)
            {
                m_event(sender, args);
            }
        }
    }
}
