using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Threading
{
    #region CancelAction

    /// <summary>
    /// Action the CancelableFuture should take when a cancel occurs
    /// </summary>
    public enum CancelAction
    {
        /// <summary>
        /// Abort the thread running the future.
        /// </summary>
        AbortThread,

        /// <summary>
        /// Do nothing.  Set the Cancelled flag on the operation context
        /// but allow the future to cancel itself
        /// </summary>
        Nothing
    }

    #endregion

    #region CancelableFuture

    /// <summary>
    /// Future operations that can be cancelled
    /// </summary>
    public abstract class CancelableFuture : Future
    {
        private object m_lock = new object();
        private OperationContext m_opContext;
        private CancelAction m_cancelAction;
        private Thread m_futureThread;
        private ProgressTrackerProxy m_proxy;

        /// <summary>
        /// Action that should be taken when a cancel occurs
        /// </summary>
        public CancelAction CancelAction
        {
            get { return m_cancelAction; }
        }

        /// <summary>
        /// Context passed to the operation
        /// </summary>
        public OperationContext OperationContext
        {
            get { return m_opContext; }
        }

        protected CancelableFuture(CancelAction action, ProgressTrackerFactory factory)
        {
            m_cancelAction = action;
            m_proxy = new ProgressTrackerProxy(this, factory);
            m_opContext = new OperationContext(m_proxy);
        }

        /// <summary>
        /// Cancel the operation.  If the operation has completed this will have no affect
        /// </summary>
        public void Cancel()
        {
            lock (m_lock)
            {
                m_opContext.Canceled = true;

                switch (m_cancelAction)
                {
                    case CancelAction.AbortThread:

                        // Two Cases here that could produce null
                        // 1) Cancel called before the future had a chance to start.  There is a thread
                        //    that is trying to run this future so set the canceled flag and
                        //    we're done.  
                        // 2) Operation already completed.  Still set the flag and back out.  Not
                        //
                        // In either case this is not an error since the desired result has
                        // been achieved
                        if (m_futureThread != null)
                        {
                            m_futureThread.Abort();

                            // Don't run a join here.  m_futureThread will be null'd out on
                            // exit so this could be null
                        }
                        break;
                    default:
                        // Do nothing by default
                        break;
                }
            }
        }

        /// <summary>
        /// Show the progress tracker blocking.  This will return when the operation completes
        /// </summary>
        public void ShowTrackerBlockingEmpty()
        {
            m_proxy.ShowTrackerBlocking();
            WaitEmpty();
        }

        /// <summary>
        /// Show the progress tracker and return immediately
        /// </summary>
        public void ShowTrackerNonBlocking()
        {
            m_proxy.ShowTrackerNonBlocking();
        }

        protected override void RunFuture()
        {
            lock (m_lock)
            {
                switch (m_cancelAction)
                {
                    case CancelAction.AbortThread:

                        // If we've already been cancelled then throw
                        if (this.OperationContext.Canceled)
                        {
                            throw CreateCancelledException();
                        }
                        break;
                    default:
                        // By default do nothing
                        break;
                }

                Interlocked.Exchange<Thread>(ref m_futureThread, Thread.CurrentThread);
            }
        }

        protected override void OnFutureCompletedCore()
        {
            base.OnFutureCompletedCore();

            lock (m_lock)
            {
                // Make sure to null out the thread.  That way if someone calls cancel
                // long after the future completes an abort won't occur
                Interlocked.Exchange<Thread>(ref m_futureThread, null);
            }

            m_proxy.OperationFinished();
        }

        /// <summary>
        /// Add a cancelled check
        /// </summary>
        protected override void EnsureCompletedSuccessfully()
        {
            if (m_opContext.Canceled)
            {
                throw CreateCancelledException();
            }

            base.EnsureCompletedSuccessfully();
        }

        private static FutureCanceledException CreateCancelledException()
        {
            return new FutureCanceledException("Operation was cancelled");
        }

        #region Static Creation Methods

        public static CancelableFuture Create(Action<OperationContext> del)
        {
            return Create(del, CancelAction.Nothing); 
        }

        public static CancelableFuture Create(Action<OperationContext> del, CancelAction action)
        {
            return Create(del, action, ProgressTrackerFactory.Default); 
        }

        public static CancelableFuture Create(Action<OperationContext> del, CancelAction action, ProgressTrackerFactory factory) 
        {
            EmptyCancelableFuture cf = new EmptyCancelableFuture(del, action, factory);
            cf.RunInThreadPool();
            return cf;
        }

        public static CancelableFuture<T> Create<T>(Func<OperationContext, T> del)
        {
            return Create(del, CancelAction.Nothing); 
        }

        public static CancelableFuture<T> Create<T>(Func<OperationContext, T> del, CancelAction action) 
        {
            return Create(del, action, ProgressTrackerFactory.Default); 
        }

        public static CancelableFuture<T> Create<T>(Func<OperationContext, T> del, CancelAction action, ProgressTrackerFactory factory)
        {
            CancelableFuture<T> cf = new CancelableFuture<T>(del, action, factory);
            cf.RunInThreadPool();
            return cf;
        }

        public static CancelableFuture CreateNoRun(Action<OperationContext> callback, CancelAction action, ProgressTrackerFactory factory)
        {
            return new EmptyCancelableFuture(callback, action, factory);
        }

        public static CancelableFuture<T> CreateNoRun<T>(Func<OperationContext, T> del, CancelAction action, ProgressTrackerFactory factory)
        {
            return new CancelableFuture<T>(del, action, factory);
        }

        #endregion
    }

    #endregion

    #region CancelableFuture<T>

    /// <summary>
    /// Cancelable future that returns a value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CancelableFuture<T> : CancelableFuture, IFuture<T>
    {
        /// <summary>
        /// Not typed to object so we can make it volatile.  Adds boxing overhead but it allows us
        /// to avoid a lock
        /// </summary>
        private volatile object m_value;
        private Func<OperationContext,T> m_futureDel;

        public T Value
        {
            get { return Wait(); }
        }

        internal CancelableFuture(Func<OperationContext, T> del, CancelAction action, ProgressTrackerFactory factory)
            : base(action, factory)
        {
            m_futureDel = del;
        }

        public T Wait()
        {
            this.WaitEmpty();
            return (T)m_value;
        }

        public T ShowTrackerBlocking()
        {
            ShowTrackerBlockingEmpty();
            return Wait();
        }

        protected override void RunFuture()
        {
            base.RunFuture();
            m_value = m_futureDel(this.OperationContext);
        }
    }

    #endregion

    #region EmptyCancelableFuture


    /// <summary>
    /// Cancelable future that returns a value
    /// </summary>
    internal sealed class EmptyCancelableFuture : CancelableFuture
    {
        private Action<OperationContext> m_futureDel;

        internal EmptyCancelableFuture(Action<OperationContext> del, CancelAction action, ProgressTrackerFactory factory)
            : base(action, factory)
        {
            m_futureDel = del;
        }

        protected override void RunFuture()
        {
            base.RunFuture();
            m_futureDel(this.OperationContext);
        }

    }

    #endregion
}
