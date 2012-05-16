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
    [Immutable]
    [Serializable]
    [SuppressMessage("Microsoft.Naming", "CA1710")]
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
    public class ImmutableLinkedList<T> : IEnumerable<T>
    {
        #region Empty

        [Immutable]
        [Serializable]
        private sealed class EmptyImmutableLinkedList : ImmutableLinkedList<T>
        {
            public override bool IsEmpty
            {
                get { return true; }
            }
            public override T Data
            {
                get { throw CreateEmptyException(); }
            }

            public override ImmutableLinkedList<T> Next
            {
                get { throw CreateEmptyException(); }
            }

            private static InvalidOperationException CreateEmptyException()
            {
                return new InvalidOperationException("List is empty");
            }
        }

        #endregion

        private static readonly EmptyImmutableLinkedList s_empty = new EmptyImmutableLinkedList();

        public static ImmutableLinkedList<T> Empty
        {
            get { return s_empty; }
        }

        private readonly T m_data;
        private readonly ImmutableLinkedList<T> m_next;

        private ImmutableLinkedList()
        {

        }

        internal ImmutableLinkedList(T data)
            : this(data, s_empty)
        {
        }

        internal ImmutableLinkedList(T data, ImmutableLinkedList<T> next)
        {
            m_data = data;
            m_next = next;
        }

        public virtual bool IsEmpty
        {
            get { return false; }
        }

        public virtual T Data
        {
            get { return m_data; }
        }

        public virtual ImmutableLinkedList<T> Next
        {
            get { return m_next; }
        }

        public ImmutableLinkedList<T> AddFront(T value)
        {
            return new ImmutableLinkedList<T>(value, this);
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            var cur = this;
            while (!cur.IsEmpty)
            {
                yield return cur.Data;
                cur = cur.Next;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }

    public static class ImmutableLinkedList
    {
        public static ImmutableLinkedList<T> Create<T>(IEnumerable<T> enumerable)
        {
            var reverse = enumerable.Reverse();
            var cur = ImmutableLinkedList<T>.Empty;
            foreach (var item in reverse)
            {
                cur = cur.AddFront(item);
            }

            return cur;
        }

        public static ImmutableLinkedList<T> CreateFromArguments<T>(params T[] args)
        {
            return Create(args);
        }
    }
}
