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
    /// Meant to be a light weight event in the sense that it does not 
    /// require a dispose
    /// </summary>
    internal sealed class ActiveOperation
    {
        private int m_hasCompleted;
        private ManualResetEvent m_event;

        /// <summary>
        /// Whether or not the operation has completed
        /// </summary>
        internal bool HasCompleted
        {
            get { return 1 == m_hasCompleted; }
        }

        internal ActiveOperation()
        {

        }

        /// <summary>
        /// Wait for the operation to complete
        /// </summary>
        internal void Wait()
        {
            // Easy case, we're already finished
            if (1 == m_hasCompleted)
            {
                return;
            }

            // Wait on an existing event if it's available
            if (null != m_event)
            {
                WaitOnEvent(m_event);
            }

            ManualResetEvent createdEvent = null;
            ManualResetEvent originalEvent = null;
            try
            {
                createdEvent = new ManualResetEvent(false);
                originalEvent = Interlocked.CompareExchange<ManualResetEvent>(ref m_event, createdEvent, null);

                // There are 2 race conditions that we can encounter at this point.
                // 1) Two separate threads created event objects.  In that case if we are the second
                //    such thread the "originalEvent" variable will be non-null.  Destroy the created
                //    event and wait on the first event
                // 2) Between our original completion check and the time we set the event OnCompleted
                //    was called.  Now that the event has been created re-check the has completed value 
                //    and destroy the event if necessary
                if (null != originalEvent)
                {
                    // Another thread got here before us.  Destroy the created event and wait on the original
                    createdEvent.Close();
                    createdEvent = null;	// So it doesn't get re-disposed

                    WaitOnEvent(originalEvent);
                }
                else if (1 == m_hasCompleted)
                {
                    createdEvent.Close();
                    createdEvent = null;	// So it doesn't get re-disposed
                }
                else
                {
                    createdEvent.WaitOne();
                }
            }
            finally
            {
                // If the created event is m_event we need to swap it out for null
                if (null == originalEvent)
                {
                    Interlocked.Exchange(ref m_event, null);
                }

                if (null != createdEvent)
                {
                    createdEvent.Close();
                }
            }
        }

        private static void WaitOnEvent(ManualResetEvent e)
        {
            try
            {
                e.WaitOne();
            }
            catch (ObjectDisposedException)
            {
                // There is a race condition where this event can get disposed
                // while we're still in a wait.  Not a problem because all we
                // care about is the event getting hit
            }
        }

        /// <summary>
        /// Called when the operation complete
        /// </summary>
        internal void Completed()
        {
            // It's possible that this is called more than once.  Take the condition
            // where we are in the middle of an OnComplete cycle, then a ThreadAbortedException
            // occurrs.  We would then move into OnErrored so guard against this.
            if (0 == Interlocked.CompareExchange(ref m_hasCompleted, 1, 0))
            {
                try
                {
                    ManualResetEvent mre = m_event;
                    if (null != mre)
                    {
                        mre.Set();
                    }
                }
                catch (ObjectDisposedException)
                {
                    // A race condition exists where we could get a reference to disposed 
                    // event so guard against it.  It's not an error since the disposer of
                    // the event is responsible for taking care of the implications of the 
                    // dipsose
                }
            }
        }
    }
}
