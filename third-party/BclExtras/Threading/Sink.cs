using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using ThreadTimer = System.Threading.Timer;

namespace BclExtras.Threading
{
    #region Sink<T>

    public delegate void SinkCallback<T>(Sink<T> sink) where T : class;

    /// <summary>
    /// Interface Sink tied to a specific thread
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Sink<T>
        where T : class
    {

        private T m_sink;
        private ISynchronizeInvoke m_invoke;

        public T ISink
        {
            get { return m_sink; }
        }

        public ISynchronizeInvoke ISynchronizeInvoke
        {
            get { return m_invoke; }
        }

        public Sink(T sink, ISynchronizeInvoke invoke)
        {
            Contract.ThrowIfNull(invoke);
            Contract.ThrowIfNull(sink);

            m_sink = sink;
            m_invoke = invoke;
        }

        public void Invoke(SinkCallback<T> callback)
        {
            SafeInvoke.Invoke(m_invoke, () => callback(this));
        }

        public IAsyncResult BeginInvoke(SinkCallback<T> callback)
        {
            return SafeInvoke.BeginInvoke(m_invoke, () => callback(this));
        }
    }

    #endregion

    #region SinkIterator<T>

    /// <summary>
    /// Used to call methods on collections of Sink
    /// </summary>
    public static class SinkIterator
    {
        public static void RunAsynchronously<T>(IEnumerable<Sink<T>> enumerable, Action<T> callback)
            where T : class
        {
            foreach (Sink<T> cur in enumerable)
            {
                SafeInvoke.BeginInvoke(cur.ISynchronizeInvoke, callback, cur.ISink);
            }
        }

        public static void RunAsynchronouslyNoThrow<T>(IEnumerable<Sink<T>> enumerable, Action<T> callback, LogItemType type, string category)
            where T : class
        {
            Action<T> wrappedCb = delegate(T cur)
            {
                try
                {
                    callback(cur);
                }
                catch (Exception ex)
                {
                    Logger.Log(type, category, ex.Message, ex);
                }
            };

            foreach (Sink<T> cur in enumerable)
            {
                try
                {
                    SafeInvoke.BeginInvoke(cur.ISynchronizeInvoke, wrappedCb, cur.ISink);
                }
                catch (Exception ex)
                {
                    Logger.Log(type, category, ex.Message, ex);
                }
            }
        }

        public static void RunSynchronously<T>(IEnumerable<Sink<T>> list, Action<T> callback)
            where T : class
        {
            foreach (Sink<T> cur in list)
            {
                SafeInvoke.Invoke(cur.ISynchronizeInvoke, callback, cur.ISink);
            }
        }

        public static void RunSynchronouslyNoThrow<T>(IEnumerable<Sink<T>> enumerable, Action<T> callback, LogItemType type, string category)
            where T : class
        {
            foreach (Sink<T> cur in enumerable)
            {
                try
                {
                    SafeInvoke.Invoke(cur.ISynchronizeInvoke, callback, cur.ISink);
                }
                catch (Exception ex)
                {
                    Logger.Log(type, category, ex.Message, ex);
                }
            }
        }

    }

    #endregion
}