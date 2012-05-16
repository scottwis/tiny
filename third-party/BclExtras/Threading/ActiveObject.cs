using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using BclExtras.Collections;

namespace BclExtras.Threading
{
    /// <summary>
    /// What to do when an ActiveObject is disposed
    /// </summary>
    public enum DisposeAction
    {
        RunRemainingActions,
        AbortRemainingActions,

        /// <summary>
        /// Dangerous, will cause the ActiveObject to abort it's background thread.  No guarantee
        /// of the state the remaining actions will be in
        /// </summary>
        AbortThread,
    }

    /// <summary>
    /// ActiveObject base class.  Lives on a separate thread
    /// </summary>
    [ThreadSafe]
    public abstract class ActiveObject : IDisposable
    {
        private const int DisposeState_None = 0;
        private const int DisposeState_Disposing = 1;
        private const int DisposeState_Disposed = 2;
            
        public const ApartmentState DefaultApartmentState = ApartmentState.MTA;

        private readonly Thread m_thread;
        private readonly DisposeAction m_action;
        private readonly List<Action> m_disposeActions = new List<Action>();
        private PipeSingleReader<Future> m_channel; 
        private volatile int m_disposeState;

        public bool IsDisposed
        {
            get { return DisposeState_Disposed == m_disposeState; }
        }

        public bool IsDisposing
        {
            get { return DisposeState_Disposing == m_disposeState; }
        }

        protected ActiveObject(DisposeAction action)
            : this(action, "ActiveObject", DefaultApartmentState)
        {

        }

        protected ActiveObject(DisposeAction action, string threadName)
            : this(action, threadName, DefaultApartmentState)
        {

        }

        protected ActiveObject(DisposeAction action, string threadName, ApartmentState apartmentState)
        {
            m_action = action;
            m_thread = new Thread((ThreadStart)delegate { this.PumpBackgroundActions(); });
            m_thread.SetApartmentState(apartmentState);
            m_thread.Name = threadName;
            m_thread.Start();
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ActiveObject()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {

                    m_disposeState = DisposeState_Disposing;
                    if (m_action == DisposeAction.AbortThread)
                    {
                        m_thread.Abort();
                    }
                    else
                    {
                        // Signal the disposed state and wake up the background thread.  Add the extra background
                        // action to ensure the background thread wakes up
                        CallOnBackground(() => { });
                        m_channel.CloseInput();
                    }

                    m_thread.Join();
                }
                finally
                {
                    m_channel.Dispose();
                    m_disposeState = DisposeState_Disposed;
                }
            }
        }

        #endregion

        #region Background Methods

        private void PumpBackgroundActions()
        {
            try
            {
                bool done = false;
                m_channel = new PipeSingleReader<Future>();
                while (!done)
                {
                    Future current = m_channel.GetNextOutput();
                    if (DisposeState_Disposing == m_disposeState)
                    {
                        done = true;
                        switch (m_action)
                        {
                            case DisposeAction.AbortRemainingActions:
                                do
                                {
                                    current.RunAborted();
                                } while (m_channel.TryGetOutput(out current));
                                break;
                            case DisposeAction.RunRemainingActions:
                                do
                                {
                                    RunSingleBackgroundDelegate(current);
                                } while (m_channel.TryGetOutput(out current));
                                break;
                            case DisposeAction.AbortThread:
                                // Small racecondition here.  In the middle of dispose after the
                                // dispose state is set but before the abort occurs we get here.  In this 
                                // case we just sleep because an abort is imminent 
                                Thread.Sleep(Timeout.Infinite);
                                break;
                            default:
                                Contract.InvalidEnumValue(m_action);
                                break;
                        }
                    }
                    else
                    {
                        RunSingleBackgroundDelegate(current);
                    }
                }
            }
            finally
            {
                // Run the dispose actions
                foreach (var cur in m_disposeActions)
                {
                    cur();
                }
            }
        }

        private void RunSingleBackgroundDelegate(Future fb)
        {
            try
            {
                fb.Run();

                Contract.ThrowIfFalse(fb.HasCompleted);
                if (fb.Error != null)
                {
                    this.OnUnhandledBackgroundException(fb.Error);
                }
            }
            catch (ThreadAbortException ex)
            {
                // If we are in the middle of a dispose that aborts the thread then it's not really
                // an unhandled exception because it's expected.
                if (DisposeAction.AbortThread != m_action || DisposeState_Disposing != m_disposeState)
                {
                    this.OnUnhandledBackgroundException(ex);
                }
                throw;
            }
            catch (Exception ex)
            {
                this.OnUnhandledBackgroundException(ex);
            }
        }

        protected internal Future AddDisposeAction(Action del)
        {
            return CallOnBackground(() => m_disposeActions.Add(del));
        }

        #endregion

        #region Core Helpers

        protected internal void CallOnBackground(Future future)
        {
            // For thread affinity reasons the Channel instance must be created on the background
            // thread.  So we have to ensure that it's been created here before enqueueing the 
            // action
            while (null == m_channel)
            {
                Thread.Sleep(0);
            }

            m_channel.AddInput(future);
        }

        protected internal Future CallOnBackground(Action del)
        {
            Future future = new EmptyFuture(del);
            this.CallOnBackground(future);
            return future;
        }

        protected internal Future<T> CallOnBackground<T>(Func<T> del)
        {
            Future<T> future = new Future<T>(del);
            this.CallOnBackground(future);
            return future;
        }

        protected internal void EnsureOnBackgroundThread()
        {
            Contract.EnsureOnThread(m_thread.ManagedThreadId);
        }

        protected internal void EnsureNotOnBackgroundThread()
        {
            Contract.EnsureNotOnThread(m_thread.ManagedThreadId);
        }

        #endregion

        #region Virtual Methods

        protected virtual void OnUnhandledBackgroundException(Exception ex)
        {
            // Swallow it by default
        }

        #endregion
    }

}
