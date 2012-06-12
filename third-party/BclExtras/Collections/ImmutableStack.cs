using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using BclExtras.Threading;
using System.Diagnostics;
#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Collections
{
    /// <summary>
    /// Immutable stack implementation
    /// 
    /// Heavily influenced by Eric Lippert's blog postings on immutable collections
    /// http://blogs.msdn.com/ericlippert
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1711")]
    [SuppressMessage("Microsoft.Naming", "CA1710")]
    [Immutable]
    [Serializable]
    [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
    public class ImmutableStack<T> : IPersistentCollection<T>, IEnumerable<T>
    {
        #region Empty

        [Immutable]
        [Serializable]
        private sealed class EmptyImmutableStack : ImmutableStack<T>
        {
            public override bool IsEmpty
            {
                get { return true; }
            }

            public override T Peek()
            {
                throw CreateEmptyException();
            }

            public override ImmutableStack<T> Pop()
            {
                throw CreateEmptyException();
            }

            private static InvalidOperationException CreateEmptyException()
            {
                return new InvalidOperationException("Stack is empty");
            }
        }

        #endregion

        private static readonly EmptyImmutableStack s_empty = new EmptyImmutableStack();

        public static ImmutableStack<T> Empty
        {
            get { return s_empty; }
        }

        private readonly int m_count;
        private readonly T m_data;
        private readonly ImmutableStack<T> m_next;

        private ImmutableStack()
        {

        }

        public ImmutableStack(T data)
            : this(data, s_empty)
        {
        }

        public ImmutableStack(T data, ImmutableStack<T> next)
        {
            m_data = data;
            m_next = next;
            m_count = next.m_count + 1;
        }

        public virtual bool IsEmpty
        {
            get { return false; }
        }

        public int Count
        {
            get { return m_count; }
        }

        public virtual T Peek()
        {
            return m_data;
        }

        public virtual ImmutableStack<T> Pop()
        {
            return m_next;
        }

        public ImmutableStack<T> Push(T data)
        {
            return new ImmutableStack<T>(data, this);
        }

        public ImmutableStack<T> Reverse()
        {
            ImmutableStack<T> r = ImmutableStack<T>.Empty;
            ImmutableStack<T> current = this;
            while (!current.IsEmpty)
            {
                r = r.Push(current.Peek());
                current = current.Pop();
            }

            return r;
        }

        public ImmutableStack<T> Remove(T value)
        {
            var stack = ImmutableStack<T>.Empty;
            var comp = EqualityComparer<T>.Default;
            bool found = false;
            foreach (var item in this)
            {
                if (!found && comp.Equals(item, value))
                {
                    found = true;
                }
                else
                {
                    stack = stack.Push(item);
                }
            }

            return stack.Reverse();
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            for (var cur = this; !cur.IsEmpty; cur = cur.Pop())
            {
                yield return cur.Peek();
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IPersistentCollection<T> Members

        IPersistentCollection<T> IPersistentCollection<T>.Add(T value)
        {
            return Push(value);
        }

        IPersistentCollection<T> IPersistentCollection<T>.Remove(T value)
        {
            return Remove(value);
        }

        IPersistentCollection<T> IPersistentCollection<T>.Clear()
        {
            return ImmutableStack<T>.Empty;
        }

        #endregion

        #region IReadOnlyCollectionEx<T> Members

        bool IReadOnlyCollectionEx<T>.Contains(T value)
        {
            var comp = EqualityComparer<T>.Default;
            return this.Any(x => comp.Equals(x, value));
        }

        #endregion
    }

    [SuppressMessage("Microsoft.Naming", "CA1711")]
    public static class ImmutableStack
    {
        public static ImmutableStack<T> CreateForSingle<T>(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            return new ImmutableStack<T>(data);
        }

        public static ImmutableStack<T> Create<T>(IEnumerable<T> enumerable)
        {
            var stack = ImmutableStack<T>.Empty;
            foreach (var item in enumerable)
            {
                stack = stack.Push(item);
            }

            return stack;
        }

    }
}
