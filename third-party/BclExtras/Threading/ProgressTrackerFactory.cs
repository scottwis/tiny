using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Threading
{
    public class ProgressTrackerFactory
    {
        [ThreadStatic]
        private static ProgressTrackerFactory s_factory;
        private static object s_lock = new object();

        public static ProgressTrackerFactory Default
        {
            get
            {
                if (s_factory == null)
                {
                    lock (s_lock)
                    {
                        if (s_factory == null)
                        {
                            s_factory = new ProgressTrackerFactory();
                        }
                    }
                }

                return s_factory;
            }
            set
            {
                s_factory = value;
            }
        }

        public static ProgressTrackerFactory Hidden
        {
            get { return new ProgressTrackerFactory(); }
        }

        #region HiddenProgress Tracker

        /// <summary>
        /// Used when there is not actually a progress dialog but one is neeeded
        /// so that a null is not passed
        /// </summary>
        internal sealed class HiddenProgressTracker : IProgressTracker
        {
            #region IProgressTracker Members

            public bool IsCancelable
            {
                get { return false; }
                set { }
            }

#pragma warning disable 67
            public event EventHandler Canceled;
#pragma warning restore 67

            public void SetTitle(string description)
            {
                // Do Nothing
            }

            public void SetSummary(string subDescription)
            {
                // Do Nothing
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

            #endregion

            #region ISynchronizeInvoke Members

            public IAsyncResult BeginInvoke(Delegate method, object[] args)
            {
                return null;
            }

            public object EndInvoke(IAsyncResult result)
            {
                return null;
            }

            public object Invoke(Delegate method, object[] args)
            {
                return null;
            }

            public bool InvokeRequired
            {
                get { return false; }
            }

            #endregion
        }

        #endregion

        /// <summary>
        /// Create an IProgressTracker on the current thread
        /// </summary>
        /// <returns></returns>
        public virtual IProgressTracker CreateProgressTracker()
        {
            return new HiddenProgressTracker();
        }

        /// <summary>
        /// Display the tracker, possible block
        /// </summary>
        /// <param name="tracker"></param>
        public virtual void Display(IProgressTracker tracker)
        {
            // Nothing to do
        }

        /// <summary>
        /// Destroy the IProgressTracker previously returned from 
        /// CreateProgressTracker.  This can be called from any thread
        /// </summary>
        /// <param name="tracker"></param>
        public virtual void DestroyProgressTracker(IProgressTracker tracker)
        {
            // Nothing to do
        }

        protected ProgressTrackerFactory()
        {

        }
    }

    public abstract class ProgressTrackerFactory<T> : ProgressTrackerFactory
        where T : IProgressTracker
    {
        public sealed override IProgressTracker CreateProgressTracker()
        {
            return CreateProgressTrackerCore();
        }

        public override void Display(IProgressTracker tracker)
        {
            T value = Contract.ThrowIfNotType<T>(tracker);
            DisplayCore(value);
        }

        public override void DestroyProgressTracker(IProgressTracker tracker)
        {
            T value = Contract.ThrowIfNotType<T>(tracker);
            DestroyProgressTrackerCore(value);
        }

        protected abstract T CreateProgressTrackerCore();

        protected abstract void DisplayCore(T tracker);

        protected abstract void DestroyProgressTrackerCore(T tracker);

    }
}