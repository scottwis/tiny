using System;
using System.Collections.Generic;
using System.Text;
using BclExtras.Threading;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Collections
{
    /// <summary>
    /// Immutable Queue structure.  Uses two stacks to avoid duplicating data at every turn.  One stack
    /// is used to return values and the other is used to add values.  When the return value stack is empty,
    /// the add value stack is reversed and moved to the return values position
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Immutable]
    [Serializable]
    [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
    public sealed class ImmutableQueue<T> : IPersistentCollection<T>
    {
        #region Static Members

        private static readonly ImmutableQueue<T> s_empty = new ImmutableQueue<T>();

        public static ImmutableQueue<T> Empty
        {
            get { return s_empty; }
        }

        #endregion

        private readonly ImmutableStack<T> m_provideStack;
        private readonly ImmutableStack<T> m_addStack;

        public int Count
        {
            get { return m_provideStack.Count + m_addStack.Count; }
        }

        public bool IsEmpty
        {
            get { return 0 == Count; }
        }

        private ImmutableQueue()
        {
            m_provideStack = ImmutableStack<T>.Empty;
            m_addStack = ImmutableStack<T>.Empty;
        }

        private ImmutableQueue(ImmutableStack<T> provideStack, ImmutableStack<T> addStack)
        {
            if (provideStack.IsEmpty)
            {
                m_provideStack = addStack.Reverse();
                m_addStack = ImmutableStack<T>.Empty;
            }
            else
            {
                m_provideStack = provideStack;
                m_addStack = addStack;
            }
        }

        public ImmutableQueue(IEnumerable<T> enumerable)
        {
            m_provideStack = ImmutableStack.Create(enumerable).Reverse();
            m_addStack = ImmutableStack<T>.Empty;
        }

        public T Peek()
        {
            return m_provideStack.Peek();
        }

        public ImmutableQueue<T> Enqueue(T value)
        {
            if (IsEmpty)
            {
                return new ImmutableQueue<T>(
                    new ImmutableStack<T>(value),
                    ImmutableStack<T>.Empty);
            }

            return new ImmutableQueue<T>(
                m_provideStack,
                m_addStack.Push(value));
        }

        public ImmutableQueue<T> Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Queue is empty");
            }
            else if (1 == Count)
            {
                return ImmutableQueue<T>.Empty;
            }
            else
            {
                return new ImmutableQueue<T>(
                    m_provideStack.Pop(),
                    m_addStack);
            }
        }

        #region IPersistentCollection<T> Members

        IPersistentCollection<T> IPersistentCollection<T>.Add(T value)
        {
            return Enqueue(value);
        }

        /// <summary>
        /// Make sure to only remove the value once
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IPersistentCollection<T> IPersistentCollection<T>.Remove(T value)
        {
            ImmutableStack<T> provide = m_provideStack.Remove(value);
            ImmutableStack<T> add = m_addStack;
            if (provide.Count == m_addStack.Count)
            {
                add = add.Remove(value);
            }

            return new ImmutableQueue<T>(provide, add);
        }

        IPersistentCollection<T> IPersistentCollection<T>.Clear()
        {
            return ImmutableQueue<T>.Empty;
        }

        #endregion

        #region IReadOnlyCollectionEx<T> Members

        int IReadOnlyCollection<T>.Count
        {
            get { return Count; }
        }

        bool IReadOnlyCollectionEx<T>.Contains(T value)
        {
            return
                ((IPersistentCollection<T>)m_provideStack).Contains(value)
                || ((IPersistentCollection<T>)m_addStack).Contains(value);
        }

        #endregion

        #region IEnumerable<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return m_provideStack.Concat(m_addStack.Reverse()).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }

        #endregion
    }

    public static class ImmutableQueue
    {
        public static ImmutableQueue<T> Create<T>(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            ImmutableQueue<T> queue = ImmutableQueue<T>.Empty;
            foreach (var cur in enumerable)
            {
                queue = queue.Enqueue(cur);
            }

            return queue;
        }

        public static ImmutableQueue<T> CreateForSingle<T>(T value)
        {
            return ImmutableQueue<T>.Empty.Enqueue(value);
        }
    }
}
