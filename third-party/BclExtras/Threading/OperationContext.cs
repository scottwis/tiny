using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Threading
{
    /// <summary>
    /// Base implementation of IOperationContext
    /// </summary>
    public sealed class OperationContext
    {
        #region EmptyProxy

        private sealed class EmptyProxy : IProgressTrackerProxy
        {
            public void SetIsCancelSupported(bool value)
            {
            }

            public void SetTitle(string title)
            {
            }

            public void SetSummary(string summary)
            {
            }

            public void SetStepInformation(int increment, int total)
            {
            }

            public void SetCurrentStep(int step)
            {
            }

            public void PerformStep()
            {
            }
        }

        #endregion

        public static OperationContext Empty
        {
            get { return new OperationContext(new EmptyProxy()); }
        }

        private IProgressTrackerProxy m_proxy;
        private bool m_cancelled;

        /// <summary>
        /// Whether or not the operation has been cancelled
        /// </summary>
        public bool Canceled
        {
            get { return m_cancelled; }
            set { m_cancelled = value; }
        }

        internal OperationContext(IProgressTrackerProxy dialog)
        {
            m_proxy = dialog;
        }

        public void SetTitle(string msg)
        {
            m_proxy.SetTitle(msg);
        }

        public void SetTitle(string format, params object[] args)
        {
            m_proxy.SetTitle(String.Format(format, args));
        }

        public void SetSummary(string msg)
        {
            m_proxy.SetSummary(msg);
        }

        public void SetSummary(string format, params object[] args)
        {
            m_proxy.SetSummary(string.Format(format, args));
        }

        public void SetIsCancelAllowed(bool value)
        {
            m_proxy.SetIsCancelSupported(value);
        }

        public void SetStepInformation(int total)
        {
            m_proxy.SetStepInformation(1, total);
        }

        public void SetStepInformation(int increment, int total)
        {
            m_proxy.SetStepInformation(increment, total);
        }

        public void PerformStep()
        {
            m_proxy.PerformStep();
        }

        public void SetCurrentStep(int current)
        {
            m_proxy.SetCurrentStep(current);
        }

        public void ThrowIfCanceled()
        {
            if (m_cancelled)
            {
                throw new OperationCanceledException("Operation was cancelled");
            }
        }
    }
}
