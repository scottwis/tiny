using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;

namespace BclExtras.Threading
{
    public class ImmediateInvoke : ISynchronizeInvoke
    {
        #region AsyncResult

        private class AsyncResult : IAsyncResult
        {
            internal ManualResetEvent Handle { get; set; }
            internal Future<object> Future { get; set; }

            internal AsyncResult(Delegate method, object[] args)
            {
                Future = BclExtras.Threading.Future.Create(() => method.DynamicInvoke(args) );
            }

            #region IAsyncResult Members

            public object AsyncState
            {
                get { return Future; }
            }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (Handle == null)
                    {
                        Handle = Future.CreateWaitHandle();
                    }

                    return Handle;
                }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return Future.HasCompleted; }
            }

            #endregion
        }

        #endregion

        #region ISynchronizeInvoke Members

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            return new AsyncResult(method, args);
        }

        public object EndInvoke(IAsyncResult result)
        {
            var r = (AsyncResult)result;
            try
            {
                r.Future.WaitEmpty();
            }
            catch (FutureException ex)
            {
                Contract.ThrowIfNull(ex.InnerException);
                throw new InvocationException("Error during BeginInvoke", ex.InnerException);
            }
            finally
            {
                if (r.Handle != null)
                {
                    r.Handle.Close();
                    r.Handle = null;
                }
            }

            return r.Future.Wait();
        }

        public object Invoke(Delegate method, object[] args)
        {
            return method.DynamicInvoke(args);
        }

        public bool InvokeRequired
        {
            get { return false; }
        }

        #endregion
    }
}
