using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using BclExtras.Collections;

#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Threading
{

    /// <summary>
    /// Creates a DataSource that is easily bindable to controls.  It will watch
    /// the future and update the list once it completes 
    /// TODO: Write Unit Tests 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class FutureDataCollection<T> : BindingList<T>
    {
        private readonly SynchronizationContext m_context;
        private readonly IEqualityComparer<T> m_comparer;
        private readonly List<Tuple<T, Future<T>>> m_pendingUpdateList = new List<Tuple<T, Future<T>>>();
        private readonly List<Future<T>> m_pendingAddList = new List<Future<T>>();

        /// <summary>
        /// Are there any pending updates
        /// </summary>
        public bool HasPendingChanges 
        {
            get { return m_pendingAddList.Count > 0 || m_pendingUpdateList.Count > 0; }
        }

        /// <summary>
        /// Raised when we start watching a future 
        /// </summary>
        public event DataEventCallback<Future<T>> FutureStarted;

        /// <summary>
        /// Raised when the future completes successfully
        /// </summary>
        public event DataEventCallback<Future<T>> FutureCompleted;

        /// <summary>
        /// Raised when the future completes but causes an error
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public event DataEventCallback<Future<T>> FutureErrored;

        public FutureDataCollection(SynchronizationContext context)
            : this(context, EqualityComparer<T>.Default)
        {

        }

        public FutureDataCollection(SynchronizationContext context, IEqualityComparer<T> comp)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (comp == null)
            {
                throw new ArgumentNullException("comp");
            }

            m_comparer = comp;
            m_context = context;
        }

        #region Public

        public void Add(Future<T> future)
        {
            AddCore(future);
        }

        public void Update(T originalValue, Future<T> future)
        {
            UpdateCore(originalValue, future);
        }

        #endregion

        #region Overrides

        protected override void ClearItems()
        {
            m_pendingAddList.Clear();
            m_pendingUpdateList.Clear();

            base.ClearItems();
        }

        #endregion

        #region Helpers

        private void AddCore(Future<T> future)
        {
            if (null != FutureStarted)
            {
                FutureStarted(this, DataEventArgs.Create(future));
            }

            m_pendingAddList.Add(future);
            future.AddCompletedCallback(m_context.BindAsPost<Future<T>>(x => OnAddCompleted(x)));
        }

        private void OnAddCompleted(Future<T> future)
        {
            ProcessFutureCompletion(future, () =>
            {
                int count = m_pendingAddList.RemoveAll(x => x == future);
                var value = future.Value;   // Get the value to force an exception on error

                // Only Add if this future is still in play
                if ( count != 0 )
                {
                    Add(future.Value);
                }
            });
        }

        private void UpdateCore(T originalValue, Future<T> future)
        {
            if (null != FutureStarted)
            {
                FutureStarted(this, DataEventArgs.Create(future));
            }

            m_pendingUpdateList.Add(Tuple.Create(originalValue, future));
            future.AddCompletedCallback(m_context.BindAsPost<Future<T>>(OnUpdateCompleted));
        }

        private void OnUpdateCompleted(Future<T> future)
        {
            ProcessFutureCompletion(future, () =>
            {
                var tuple = m_pendingUpdateList.Where(x => x.Second == future).FirstOrDefault();
                m_pendingUpdateList.RemoveAll(x => x.Second == future);

                // Get the value before removing the original item.  That way if an exception occurred in 
                // the future we will not remove the original value
                var newValue = future.Value;

                // If there is not a pending future then don't perform any updates
                if (null == tuple)
                {
                    return;
                }

                // See if there is a value to update
                var found = this.Where(x => m_comparer.Equals(x, tuple.First));
                if (!found.Any())
                {
                    return;
                }

                var index = IndexOf(found.First());
                RemoveAt(index);
                Insert(index, newValue);
            });
        }

        private void ProcessFutureCompletion(Future<T> future, Action del)
        {
            try
            {
                del();

                if (null != FutureCompleted)
                {
                    FutureCompleted(this, DataEventArgs.Create(future));
                }
            }
            catch (FutureException)
            {
                if (null != FutureErrored)
                {
                    FutureErrored(this, DataEventArgs.Create(future));
                }
            }
        }

        #endregion
    }
}
