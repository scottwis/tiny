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
    /// ActiveObject implemtation which supports dispatches into it's thread
    /// </summary>
    [ThreadSafe]
    public abstract class DispatchActiveObject : ActiveObject, ISynchronizeInvoke
    {
        private readonly ActiveObjectSynchronizationContext m_context;

        public ActiveObjectSynchronizationContext SynchronizationContext
        {
            get { return m_context; }
        }

        protected DispatchActiveObject(DisposeAction action)
            : this(action, "DispatchActiveObject", ActiveObject.DefaultApartmentState)
        {

        }

        protected DispatchActiveObject(DisposeAction action, string threadName, ApartmentState apartmentState)
            : base(action, threadName, apartmentState)
        {
            m_context = new ActiveObjectSynchronizationContext(this);
            Thread.MemoryBarrier();
            CallOnBackground(() => System.Threading.SynchronizationContext.SetSynchronizationContext(m_context)).WaitEmpty();
            AddDisposeAction(() => System.Threading.SynchronizationContext.SetSynchronizationContext(null)).WaitEmpty();
        }

        #region Public

        public Future Run(Action callback)
        {
            return CallOnBackground(callback);
        }

        public Future<T> Run<T>(Func<T> operation)
        {
            return CallOnBackground(operation);
        }

        public void Run(Future future)
        {
            CallOnBackground(future);
        }

        #endregion

        #region ISynchronizeInvoke Members

        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            var future = CallOnBackground(() => method.DynamicInvoke(args));
            return new ActiveObjectAsyncResult(future);
        }

        object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
        {
            ActiveObjectAsyncResult far = result as ActiveObjectAsyncResult;
            Contract.ThrowIfNull(far);
            
            object r = null;
            try
            {
                r = far.Future.Wait();
            }
            finally
            {
                far.DestroyWaitHandle();
            }

            return r;
        }

        object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
        {
            Future<object> f = this.CallOnBackground(() => method.DynamicInvoke(args));
            return f.Wait();
        }

        bool ISynchronizeInvoke.InvokeRequired
        {
            get { return true; }
        }

        #endregion
    }
}
