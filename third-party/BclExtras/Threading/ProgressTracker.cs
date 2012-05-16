using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Threading
{
    #region IProgressTracker

    /// <summary>
    /// TODO: Need a wait to slip in WaitUntilShow()
    /// </summary>
    public interface IProgressTracker : ISynchronizeInvoke
    {
        bool IsCancelable
        {
            get;
            set;
        }

        event EventHandler Canceled;

        void SetTitle(string description);

        void SetSummary(string subDescription);

        void SetStepInformation(int increment, int count);

        void SetCurrentStep(int current);

        void PerformStep();
    }

    #endregion

    #region IProgressTrackerProxy

    /// <summary>
    /// Interface for classes written to by an OperationContext
    /// </summary>
    internal interface IProgressTrackerProxy
    {
        void SetIsCancelSupported(bool value);
        void SetTitle(string title);
        void SetSummary(string summary);
        void SetStepInformation(int increment, int total);
        void SetCurrentStep(int step);
        void PerformStep();
    }

    #endregion

    #region ProgressTrackerProxy

    internal sealed class ProgressTrackerProxy : IProgressTrackerProxy
    {
        private struct TrackerData
        {
            public string Title;
            public string Summary;
            public bool? IsCancelAllowed;
            public int? StepTotal;
            public int? StepIncrement;
            public int? CurrentStep;
            public int? PerformStepCount;
        }

        private object m_lock = new object();
        private TrackerData m_data;
        private bool m_finished;
        private ProgressTrackerFactory m_factory;
        private IProgressTracker m_tracker;
        private CancelableFuture m_cancelableFuture;

        internal ProgressTrackerProxy(CancelableFuture cfBase, ProgressTrackerFactory factory)
        {
            m_factory = factory;
            m_cancelableFuture = cfBase;
        }

        #region Write from CancelableFuture

        void IProgressTrackerProxy.SetIsCancelSupported(bool value)
        {
            lock (m_lock)
            {
                m_data.IsCancelAllowed = value;
                UpdateTracker();
            }
        }

        void IProgressTrackerProxy.SetTitle(string title)
        {
            lock (m_lock)
            {
                m_data.Title = title;
                UpdateTracker();
            }
        }

        void IProgressTrackerProxy.SetSummary(string summary)
        {
            lock (m_lock)
            {
                m_data.Summary = summary;
                UpdateTracker();
            }
        }

        void IProgressTrackerProxy.SetStepInformation(int increment, int total)
        {
            lock (m_lock)
            {
                m_data.StepIncrement = increment;
                m_data.StepTotal = total;
                UpdateTracker();
            }
        }

        void IProgressTrackerProxy.SetCurrentStep(int step)
        {
            lock (m_lock)
            {
                m_data.CurrentStep = step;
                UpdateTracker();
            }
        }

        void IProgressTrackerProxy.PerformStep()
        {
            lock (m_lock)
            {
                if (!m_data.PerformStepCount.HasValue)
                {
                    m_data.PerformStepCount = 1;
                }
                else
                {
                    m_data.PerformStepCount += 1;
                }
                UpdateTracker();
            }
        }

        private void UpdateTracker()
        {
            if (m_tracker != null && !m_finished)
            {
                // Do a BeginInvoke here because the Invoke operation 
                // can very likely block.  Avoid that by using BeginInvoke 
                // 
                // Since we'll use the current values anyways, don't post multiple BeginInvokes onto
                // the tracker or we could overload it.  Instead post one at a time.
                try
                {
                    SafeInvoke.BeginInvoke(m_tracker, new Action(UpdateTrackerDirect));
                }
                catch (Exception ex)
                {
                    Logger.Error(FutureLogCategory.BeginInvoke, ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Run on the tracker thread 
        /// </summary>
        private void UpdateTrackerDirect()
        {
            // Copy the current data and reset the update pending flag.  Reseting the flag indicates that
            // any writes to the data now need to force another update to make it to the tracker
            TrackerData copy;
            lock (m_lock)
            {
                copy = m_data;
                m_data = new TrackerData();
            }

            UpdateTrackerDirectCore(copy, m_tracker);
        }

        private static void UpdateTrackerDirectCore(TrackerData data, IProgressTracker tracker)
        {
            if (data.Title != null)
            {
                tracker.SetTitle(data.Title);
            }

            if (data.Summary != null)
            {
                tracker.SetSummary(data.Summary);
            }

            if (data.IsCancelAllowed != null)
            {
                tracker.IsCancelable = data.IsCancelAllowed.Value;
            }

            if (data.StepTotal != null && data.StepIncrement != null)
            {
                tracker.SetStepInformation(data.StepIncrement.Value, data.StepTotal.Value);
            }

            if (data.CurrentStep != null)
            {
                tracker.SetCurrentStep(data.CurrentStep.Value);
            }

            if (data.PerformStepCount != null)
            {
                for (int i = 0; i < data.PerformStepCount.Value; ++i)
                {
                    tracker.PerformStep();
                }
            }

        }

        #endregion

        internal void OperationFinished()
        {
            lock (m_lock)
            {
                m_finished = true;

                // Close the tracker if it's open.  Make sure to invoke the close so
                // that it occurs on the proper thread
                if (m_tracker != null)
                {
                    // A race condition can occur as to who shuts down first so make sure to swallow
                    // that exception here
                    SafeInvoke.BeginInvokeNoThrow(
                        m_tracker,
                        () => m_factory.DestroyProgressTracker(m_tracker));
                }
            }
        }

        internal void ShowTrackerBlocking()
        {
            lock (m_lock)
            {
                if (m_finished)
                {
                    return;
                }

                m_tracker = m_factory.CreateProgressTracker();
                m_tracker.Canceled += new EventHandler(this.OnTrackerCanceled);
            }

            // Do the initial update
            UpdateTracker();

            try
            {
                // Call this out side the lock since Display is allowed to block.
                // Also catch errors because there is a race condition that can
                // cause m_tracker to be destroyed before being displayed.  
                m_factory.Display(m_tracker);
            }
            catch (Exception ex)
            {
                Logger.Trace("Error caught while displaying ProgressTracker", ex);
            }
        }

        internal void OnTrackerCanceled(object sender, EventArgs e)
        {
            // If we've already finished just before the user hits cancel then we 
            // ignore the cancel
            lock (m_lock)
            {
                if (m_finished)
                {
                    return;
                }
            }

            m_cancelableFuture.Cancel();
        }

        internal void ShowTrackerNonBlocking()
        {
            ThreadPool.QueueUserWorkItem(delegate { ShowTrackerBlocking(); });
        }
    }

    #endregion
}
