using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;


namespace BclExtras.Threading
{
    #region Future

    /// <summary>
    /// Represents an operation executing concurrently
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001", Justification = "CreateWaitHandle returns to the caller for freeing")]
    public abstract class Future : IFuture
    {
        private readonly ActiveOperation m_op;
        private readonly object m_cbListLock = new object();
        private List<Action<Future>> m_cbList;
        private int m_run;
        private Exception m_error;
        private int m_aborted;

        /// <summary>
        /// Has the Future started running 
        /// </summary>
        public bool HasStarted
        {
            get { return 1 == m_run; }
        }

        /// <summary>
        /// Whether or not the future has completed
        /// </summary>
        public bool HasCompleted
        {
            get { return m_op.HasCompleted; }
        }

        internal Exception Error
        {
            get { return m_error; }
        }

        internal Future()
        {
            m_op = new ActiveOperation();
        }

        #region Public

        /// <summary>
        /// Run this future in the thread pool
        /// </summary>
        public void RunInThreadPool()
        {
            CheckForDoubleRun();
            ThreadPool.QueueUserWorkItem((x) => RunFutureWrapper(), null);
        }

        /// <summary>
        /// Run the future immediately
        /// </summary>
        public void Run()
        {
            CheckForDoubleRun();
            RunFutureWrapper();
        }

        /// <summary>
        /// Called when the running of a Future is aborted for whatever reason
        /// </summary>
        public void RunAborted()
        {
            CheckForDoubleRun();
            Interlocked.Exchange(ref m_aborted, 1);
            m_op.Completed();
            OnFutureCompleted();
        }

        /// <summary>
        /// Waits for the ActiveOperation to complete and throws as necessary
        /// </summary>
        public void WaitEmpty()
        {
            m_op.Wait();

            EnsureCompletedSuccessfully();
            if (m_error != null)
            {
                throw new FutureException(m_error.Message, m_error);
            }
            else if (0 != m_aborted)
            {
                throw new FutureAbortedException("Future was aborted before being run");
            }
        }

        /// <summary>
        /// Creates a WaitHandle which will be set once the future is completed.  No
        /// caching is involved here and it's the callers responsibility to free up
        /// the handle
        /// </summary>
        /// <returns></returns>
        public ManualResetEvent CreateWaitHandle()
        {
            var handle = new ManualResetEvent(false);
            Action<Future> del = delegate
            {
                try
                {
                    handle.Set();
                }
                catch (ObjectDisposedException)
                {
                    // No need to error out if the caller has already freed up 
                    // the event
                }
            };
            AddCompletedCallback(del);
            return handle;
        }

        /// <summary>
        /// Add a delegate to be called once the future is completed.  If the future is 
        /// already completed this may be invoked immediately
        /// </summary>
        /// <param name="callback"></param>
        public void AddCompletedCallback(Action<Future> callback)
        {
            Contract.ThrowIfNull(callback);

            bool runnow = false;
            lock (m_cbListLock)
            {
                if (HasCompleted)
                {
                    runnow = true;
                }
                else
                {
                    if (m_cbList == null)
                    {
                        m_cbList = new List<Action<Future>>();
                    }
                    m_cbList.Add(callback);
                }
            }

            if (runnow)
            {
                callback(this);
            }
        }

        #endregion

        #region Internal

        #endregion

        #region Protected

        /// <summary>
        /// Used in a derived class to actually run a Future
        /// </summary>
        protected abstract void RunFuture();


        protected virtual void OnFutureCompletedCore()
        {
            // Nothing by default
        }

        protected virtual void EnsureCompletedSuccessfully()
        {
            // Nothing by default
        }

        #endregion

        #region Private

        private void RunFutureWrapper()
        {
            try
            {
                RunFuture();
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref m_error, ex);
            }
            finally
            {
                m_op.Completed();
                OnFutureCompleted();
            }
        }

        private void CheckForDoubleRun()
        {
            if (0 != Interlocked.CompareExchange(ref m_run, 1, 0))
            {
                throw new InvalidOperationException("Multiple calls to Run are not allowed.");
            }
        }

        /// <summary>
        /// Called when the future finishes running
        /// </summary>
        private void OnFutureCompleted()
        {
            Contract.ThrowIfFalse(HasCompleted);

            List<Action<Future>> cloneList = null;
            lock (m_cbListLock)
            {
                if (m_cbList != null)
                {
                    cloneList = new List<Action<Future>>(m_cbList);
                    m_cbList.Clear();
                }
            }

            if (cloneList != null)
            {
                foreach (var cb in cloneList)
                {
                    cb(this);
                }
            }

            OnFutureCompletedCore();
        }

        #endregion

        #region IFuture Members

        bool IFuture.HasCompleted
        {
            get { return this.HasCompleted; }
        }

        void IFuture.WaitEmpty()
        {
            this.WaitEmpty();
        }

        #endregion

        #region Static Creation Methods

        /// <summary>
        /// Create a future for the specified delegate
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        public static Future Create(Action del)
        {
            Future future = new EmptyFuture(del);
            future.RunInThreadPool();
            return future;
        }

        public static Future Create<TArg1>(Action<TArg1> del, TArg1 value)
        {
            Action fdel = delegate { del(value); };
            return Create(fdel);
        }

        public static Future Create<TArg1, TArg2>(Action<TArg1, TArg2> del, TArg1 value1, TArg2 value2)
        {
            Action fdel = delegate { del(value1, value2); };
            return Create(fdel);
        }

        public static Future<T> Create<T>(Func<T> del)
        {
            Future<T> future = new Future<T>(del);
            future.RunInThreadPool();
            return future;
        }

        public static Future<TReturn> Create<TArg1,TReturn>(Func<TArg1, TReturn> del, TArg1 value)
        {
            Func<TReturn> fdel = delegate { return del(value); };
            return Create(fdel);
        }

        public static Future<TReturn> Create<TArg1, TArg2, TReturn>(Func<TArg1, TArg2,TReturn> del, TArg1 value1, TArg2 value2)
        {
            Func<TReturn> fdel = delegate { return del(value1, value2); };
            return Create(fdel);
        }

        public static Future CreateNoRun(Action del)
        {
            return new EmptyFuture(del);
        }

        public static Future<T> CreateNoRun<T>(Func<T> function)
        {
            return new Future<T>(function);
        }


        #endregion
    }

    #endregion

    #region EmptyFuture

    /// <summary>
    /// Future operation that has no return value
    /// </summary>
    internal sealed class EmptyFuture : Future
    {
        private readonly Action m_futureDel;

        internal EmptyFuture(Action del)
        {
            m_futureDel = del;
        }

        protected override void RunFuture()
        {
            m_futureDel();
        }

    }

    #endregion

    #region Future<T>

    /// <summary>
    /// Future operation that returns a value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Future<T> : Future, IFuture<T>
    {
        /// <summary>
        /// Not typed to object so we can make it volatile.  Adds boxing overhead but it allows us
        /// to avoid a lock
        /// </summary>
        private volatile object m_value;
        private readonly Func<T> m_futureDel;

        /// <summary>
        /// Result of the future
        /// </summary>
        public T Value
        {
            get { return Wait(); }
        }

        internal Future(Func<T> del)
        {
            m_futureDel = del;
        }

        /// <summary>
        /// Wait for the operation to complete and return the result.  Throws if an
        /// error is encountered
        /// </summary>
        /// <returns></returns>
        public T Wait()
        {
            this.WaitEmpty();
            return (T)m_value;
        }

        public void AddCompletedCallback(Action<Future<T>> callback)
        {
            Action<Future> del = (x) => callback((Future<T>)x);
            ((Future)this).AddCompletedCallback(del);
        }

        protected override void RunFuture()
        {
            m_value = m_futureDel();
        }

    }

    #endregion

}
