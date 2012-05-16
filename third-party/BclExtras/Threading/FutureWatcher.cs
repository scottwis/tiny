using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using ThreadTimer = System.Threading.Timer;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace BclExtras.Threading
{
    #region FutureWatcher

    /// <summary>
    /// Base class for watching a future
    /// </summary>
    public abstract class FutureWatcher
    {
        #region EmptyFutureWatcher

        private sealed class EmptyFutureWatcher : FutureWatcher
        {
            private readonly Future m_wf;
            private readonly Action<FutureWatcherCompleted> m_callback;

            internal EmptyFutureWatcher(SynchronizationContext context, Future future, Action<FutureWatcherCompleted> callback)
                :base(context, future)
            {
                m_wf = future;
                m_callback = callback;
            }

            protected override void RaiseCallback(bool completed, bool cancelled)
            {
                var e = new FutureWatcherCompleted(
                    completed,
                    cancelled,
                    m_wf,
                    completed ? m_wf.Error : null,
                    this);
                m_callback(e);
            }
        }

        #endregion

        #region FutureWatcher<T>

        private sealed class TypedFutureWatcher<T> : FutureWatcher
        {
            private readonly Future<T> m_wf;
            private readonly Action<FutureWatcherCompleted<T>> m_callback;

            internal TypedFutureWatcher(SynchronizationContext context, Future<T> wf, Action<FutureWatcherCompleted<T>> callback)
                : base(context, wf)
            {
                m_wf = wf;
                m_callback = callback;
            }

            protected override void RaiseCallback(bool completed, bool cancelled)
            {
                var e = new FutureWatcherCompleted<T>(
                    completed,
                    cancelled,
                    m_wf,
                    completed ? m_wf.Error : null,
                    this);
                m_callback(e);
            }
        }

        #endregion

        private enum State
        {
            Waiting,
            Canceled,
            Completed
        }

        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        protected readonly SynchronizationContext m_context;
        [SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        protected readonly ThreadAffinity m_affinity = new ThreadAffinity();
        private readonly Future m_future;
        private State m_state;

        /// <summary>
        /// Whether or not the watcher has completed it's operations
        /// </summary>
        public bool HasCompleted
        {
            get
            {
                m_affinity.Check();
                return (State.Completed == m_state || State.Canceled == m_state);
            }
        }

        /// <summary>
        /// Whether or not the Future was completed or the watch was cancelled
        /// </summary>
        public bool HasWatchBeenCanceled
        {
            get
            {
                m_affinity.Check();
                return State.Canceled == m_state;
            }
        }

        /// <summary>
        /// Future in question
        /// </summary>
        public Future Future
        {
            get { return m_future; }
        }

        internal FutureWatcher(SynchronizationContext context, Future future)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (future == null)
            {
                throw new ArgumentNullException("future");
            }

            m_context = context;
            m_future = future;
            m_future.AddCompletedCallback(OnWatchableFutureCompleted);
            m_state = State.Waiting;
        }

        public void CancelWatch()
        {
            m_affinity.Check();
            switch (m_state)
            {
                case State.Completed:
                    // No need to do anything in this state.  We've already raised an event
                    break;
                case State.Canceled:
                    // Double cancel doesn't raise an event
                    break;
                case State.Waiting:
                    m_state = State.Canceled;
                    RaiseCallback(false, true);
                    break;
                default:
                    Contract.InvalidEnumValue(m_state);
                    break;
            }
        }

        private void OnWatchableFutureCompleted(Future future)
        {
            m_context.Post(() => OnWatchableFutureCompletedCore());
        }

        private void OnWatchableFutureCompletedCore()
        {
            if ( State.Waiting != m_state )
            {
                return;
            }

            m_state = State.Completed;
            RaiseCallback(true, false);
        }

        [SuppressMessage("Microsoft.Design", "CA1030")]
        protected abstract void RaiseCallback(bool completed, bool canceled);

        #region Static Methods

        public static FutureWatcher Create(SynchronizationContext context, Future future, Action<FutureWatcherCompleted> callback)
        {
            return new EmptyFutureWatcher(context, future, callback);
        }

        public static FutureWatcher Create<T>(SynchronizationContext context, Future<T> future, Action<FutureWatcherCompleted<T>> callback)
        {
            return new TypedFutureWatcher<T>(context, future, callback);
        }

        #endregion
    }

    #endregion

    #region FutureCompletedEventArgs

    public class FutureWatcherCompleted
    {
        private bool m_completed;
        private bool m_cancelled;
        private Exception m_exception;
        private Future m_future;
        private FutureWatcher m_watcher;

        public bool Completed
        {
            get { return m_completed; }
        }

        public bool WatchCanceled
        {
            get { return m_cancelled; }
        }

        public Future Future
        {
            get { return m_future; }
        }

        public Exception Exception
        {
            get { return m_exception; }
        }

        public FutureWatcher FutureWatcher
        {
            get { return m_watcher; }
        }

        internal FutureWatcherCompleted(bool completed, bool cancelled, Future future, Exception ex, FutureWatcher watcher)
        {
            m_completed = completed;
            m_cancelled = cancelled;
            m_exception = ex;
            m_future = future;
            m_watcher = watcher;
        }
    }

    public sealed class FutureWatcherCompleted<T> : FutureWatcherCompleted
    {
        private Future<T> m_future;

        public Future<T> FutureTyped
        {
            get { return m_future; }
        }

        internal FutureWatcherCompleted(bool success, bool cancelled, Future<T> future, Exception ex, FutureWatcher watcher)
            : base(success, cancelled, future, ex, watcher)
        {
            m_future = future;
        }
    }

    #endregion
}