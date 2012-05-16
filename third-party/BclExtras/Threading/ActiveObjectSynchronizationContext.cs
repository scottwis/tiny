using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace BclExtras.Threading
{
    [ThreadSafe]
    public sealed class ActiveObjectSynchronizationContext : SynchronizationContext
    {
        private ActiveObject m_activeObject;

        public ActiveObjectSynchronizationContext(ActiveObject activeObject)
        {
            if (activeObject == null)
            {
                throw new ArgumentNullException("activeObject");
            }

            m_activeObject = activeObject;
        }

        public override SynchronizationContext CreateCopy()
        {
            return new ActiveObjectSynchronizationContext(m_activeObject);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            m_activeObject.CallOnBackground(() => d(state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            m_activeObject.CallOnBackground(() => d(state)).WaitEmpty();
        }
    }
}
