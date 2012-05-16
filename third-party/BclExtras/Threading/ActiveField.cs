using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Threading
{
    /// <summary>
    /// Field in an active object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ActiveField<T>
    {
        /// <summary>
        /// Don't use an object here.  Interlocking isn't important since the value is primarily 
        /// accessed via Future's.
        /// </summary>
        private T m_value;

        /// <summary>
        /// ActiveObject hosting this ActiveField
        /// </summary>
        private ActiveObject m_activeObject;

        /// <summary>
        /// Get or set the value for the foreground
        /// </summary>
        [DebuggerHidden()]
        public Future<T> Value
        {
            get { return m_activeObject.CallOnBackground<T>(delegate { return m_value; }); }
        }

        /// <summary>
        /// Directly obtain the value.  Only available on the background thread
        /// </summary>
        [DebuggerHidden()]
        public T ValueDirect
        {
            get { m_activeObject.EnsureOnBackgroundThread(); return m_value; }
            set { m_activeObject.EnsureOnBackgroundThread(); m_value = value; }
        }

        public ActiveField(ActiveObject ao)
            : this(ao, default(T))
        {

        }

        public ActiveField(ActiveObject activeObject, T initialValue)
        {
            m_value = initialValue;
            m_activeObject = activeObject;
        }

        [SuppressMessage("Microsoft.Design", "CA1024")]
        public void SetValueDelayed(T value)
        {
            m_activeObject.EnsureNotOnBackgroundThread();
            m_activeObject.CallOnBackground(delegate { m_value = value; });
        }
    }
}
