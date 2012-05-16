using System;
using System.Collections.Generic;
using BclExtras.Threading;
using System.Diagnostics;

namespace BclExtras.Collections
{
    /// <summary>
    /// Adapted from Eric Lippert's double ended immutable queue solution
    /// http://blogs.msdn.com/ericlippert/archive/2008/02/12/immutability-in-c-part-eleven-a-working-double-ended-queue.aspx
    /// </summary>
    [Immutable]
    [Serializable]
    [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
    public abstract class ImmutableCollection<T> : IPersistentCollection<T>
    {
        #region MiniCollection

        [Serializable]
        protected abstract class MiniCollection : IEnumerable<T>
        {
            internal virtual bool IsFull { get { return false; } }
            internal abstract int Size { get; }
            internal abstract T Front { get; }
            internal abstract T Back { get; } 
            internal abstract MiniCollection AddFront(T value);
            internal abstract MiniCollection AddBack(T value);
            internal abstract MiniCollection RemoveFront();
            internal abstract MiniCollection RemoveBack();
            public abstract IEnumerator<T> GetEnumerator();

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion
        }

        [Immutable]
        [Serializable]
        private sealed class One : MiniCollection
        {
            private readonly T m_item1;

            internal One(T item1)
            {
                m_item1 = item1;
            }

            internal override int Size { get { return 1; } }
            internal override T Front { get { return m_item1; } }
            internal override T Back { get { return m_item1; } }
            internal override MiniCollection AddFront(T value) { return new Two(value, m_item1); }
            internal override MiniCollection AddBack(T value) { return new Two(m_item1, value); }
            internal override MiniCollection RemoveFront()
            {
                Contract.Violation("Not Possible");
                return null;
            }
            internal override MiniCollection RemoveBack()
            {
                Contract.Violation("Not Possible");
                return null;
            }
            public override IEnumerator<T> GetEnumerator()
            {
                yield return m_item1;
            }
        }

        [Immutable]
        [Serializable]
        private sealed class Two : MiniCollection
        {
            private readonly T m_item1;
            private readonly T m_item2;

            internal Two(T item1, T item2)
            {
                m_item1 = item1;
                m_item2 = item2;
            }

            internal override int Size { get { return 2; } }
            internal override T Front { get { return m_item1; }}
            internal override T Back { get { return m_item2; } }
            internal override MiniCollection AddFront(T value) { return new Three(value, m_item1, m_item2); }
            internal override MiniCollection AddBack(T value) { return new Three(m_item1, m_item2, value); }
            internal override MiniCollection RemoveFront() { return new One(m_item2); }
            internal override MiniCollection RemoveBack() { return new One(m_item1); }
            public override IEnumerator<T> GetEnumerator()
            {
                yield return m_item1;
                yield return m_item2;
            }
        }

        [Immutable]
        [Serializable]
        private sealed class Three : MiniCollection
        {
            private readonly T m_item1;
            private readonly T m_item2;
            private readonly T m_item3;

            internal Three(T item1, T item2, T item3)
            {
                m_item1 = item1;
                m_item2 = item2;
                m_item3 = item3;
            }

            internal override int Size { get { return 3; } }
            internal override T Front { get { return m_item1; } }
            internal override T Back { get {return m_item3; }}
            internal override MiniCollection AddFront(T value) { return new Four(value, m_item1, m_item2, m_item3); }
            internal override MiniCollection AddBack(T value) { return new Four(m_item1, m_item2, m_item3, value); }
            internal override MiniCollection RemoveFront() { return new Two(m_item2, m_item3); }
            internal override MiniCollection RemoveBack() { return new Two(m_item1, m_item2); }
            public override IEnumerator<T> GetEnumerator()
            {
                yield return m_item1;
                yield return m_item2;
                yield return m_item3;
            }
        }

        [Immutable]
        [Serializable]
        private sealed class Four : MiniCollection
        {
            private readonly T m_item1;
            private readonly T m_item2;
            private readonly T m_item3;
            private readonly T m_item4;

            internal Four(T item1, T item2, T item3, T item4)
            {
                m_item1 = item1;
                m_item2 = item2;
                m_item3 = item3;
                m_item4 = item4;
            }

            internal override bool IsFull { get { return true; } }
            internal override int Size { get { return 4; } }
            internal override T Front { get { return m_item1; } }
            internal override T Back { get { return m_item4; }}
            internal override MiniCollection AddFront(T value)
            {
                Contract.Violation("Not Possible");
                return null;
            }
            internal override MiniCollection AddBack(T value)
            {
                Contract.Violation("Not Possible");
                return null;
            }
            internal override MiniCollection RemoveFront() { return new Three(m_item2, m_item3, m_item4); }
            internal override MiniCollection RemoveBack() { return new Three(m_item1, m_item2, m_item3); }
            public override IEnumerator<T> GetEnumerator()
            {
                yield return m_item1;
                yield return m_item2;
                yield return m_item3;
                yield return m_item4;
            }
        }

        #endregion

        #region EmptyCollection

        [Immutable]
        [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
        [Serializable]
        private sealed class EmptyCollection : ImmutableCollection<T>
        {
            public override T Front
            {
                get { throw CreateEmptyException(); }
            }

            public override T Back
            {
                get { throw CreateEmptyException(); }
            }

            public override bool IsEmpty
            {
                get { return true; }
            }

            public override int Count
            {
                get { return 0; }
            }

            internal EmptyCollection()
            {

            }

            private static InvalidOperationException CreateEmptyException()
            {
                return new InvalidOperationException("Collection is Empty");
            }

            public override ImmutableCollection<T> AddFront(T value)
            {
                return new SingleCollection(value);
            }

            public override ImmutableCollection<T> AddBack(T value)
            {
                return new SingleCollection(value);
            }

            public override ImmutableCollection<T> RemoveFront()
            {
                throw CreateEmptyException();
            }

            public override ImmutableCollection<T> RemoveBack()
            {
                throw CreateEmptyException();
            }

            public override IEnumerator<T> GetEnumerator()
            {
                yield break;
            }
        }

        #endregion

        #region SingleCollection

        [Immutable]
        [Serializable]
        [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
        private sealed class SingleCollection : ImmutableCollection<T>
        {
            private readonly T m_value;

            public override T Front
            {
                get { return m_value; }
            }

            public override T Back
            {
                get { return m_value; }
            }

            public override int Count
            {
                get { return 1; }
            }

            public override bool IsEmpty
            {
                get { return false; }
            }

            internal SingleCollection(T value)
            {
                m_value = value;
            }

            public override ImmutableCollection<T> AddFront(T value)
            {
                return new NormalCollection(new One(value), ImmutableCollection<MiniCollection>.Empty, new One(m_value));
            }

            public override ImmutableCollection<T> AddBack(T value)
            {
                return new NormalCollection(new One(m_value), ImmutableCollection<MiniCollection>.Empty, new One(value));
            }

            public override ImmutableCollection<T> RemoveFront()
            {
                return ImmutableCollection<T>.Empty;
            }

            public override ImmutableCollection<T> RemoveBack()
            {
                return ImmutableCollection<T>.Empty;
            }

            public override IEnumerator<T> GetEnumerator()
            {
                yield return m_value;
            }
        }

        #endregion

        #region NormalCollection

        [Immutable]
        [Serializable]
        [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
        private sealed class NormalCollection : ImmutableCollection<T>
        {
            private MiniCollection m_left;
            private ImmutableCollection<MiniCollection> m_middle;
            private MiniCollection m_right;

            public override bool IsEmpty
            {
                get { return false; }
            }

            public override int Count
            {
                get
                {
                    int count = m_left.Size + m_right.Size;
                    foreach (var value in m_middle)
                    {
                        count += value.Size;
                    }

                    return count;
                }
            }

            public override T Front
            {
                get { return m_left.Front; }
            }

            public override T Back
            {
                get { return m_right.Back; }
            }

            internal NormalCollection(MiniCollection left, ImmutableCollection<MiniCollection> middle, MiniCollection right)
            {
                m_left = left;
                m_middle = middle;
                m_right = right;
            }

            public override ImmutableCollection<T> AddFront(T value)
            {
                if (m_left.IsFull)
                {
                    return new NormalCollection(
                        new Two(value, m_left.Front),
                        m_middle.AddFront(m_left.RemoveFront()),
                        m_right);
                }
                else
                {
                    return new NormalCollection(m_left.AddFront(value), m_middle, m_right);
                }
            }

            public override ImmutableCollection<T> AddBack(T value)
            {
                if (m_right.IsFull)
                {
                    return new NormalCollection(
                        m_left,
                        m_middle.AddBack(m_right.RemoveBack()),
                        new Two(m_right.Back, value));
                }
                else
                {
                    return new NormalCollection(m_left, m_middle, m_right.AddBack(value));
                }
            }

            public override ImmutableCollection<T> RemoveFront()
            {
                if (m_left.Size > 1)
                {
                    return new NormalCollection(m_left.RemoveFront(), m_middle, m_right);
                }
                else if (!m_middle.IsEmpty)
                {
                    return new NormalCollection(m_middle.Front, m_middle.RemoveFront(), m_right);
                }
                else if (m_right.Size > 1)
                {
                    return new NormalCollection(new One(m_right.Front), ImmutableCollection<MiniCollection>.Empty, m_right.RemoveFront());
                }
                else
                {
                    return new SingleCollection(m_right.Front);
                }
            }

            public override ImmutableCollection<T> RemoveBack()
            {
                if (m_right.Size > 1)
                {
                    return new NormalCollection(m_left, m_middle, m_right.RemoveBack());
                }
                else if (!m_middle.IsEmpty)
                {
                    return new NormalCollection(m_left, m_middle.RemoveBack(), m_middle.Back);
                }
                else if (m_left.Size > 1)
                {
                    return new NormalCollection(m_left.RemoveBack(), ImmutableCollection<MiniCollection>.Empty, new One(m_left.Back));
                }
                else
                {
                    return new SingleCollection(m_left.Back);
                }
            }

            public override IEnumerator<T> GetEnumerator()
            {
                foreach (var cur in m_left)
                {
                    yield return cur;
                }
                foreach (var col in m_middle)
                {
                    foreach (var cur in col)
                    {
                        yield return cur;
                    }
                }
                foreach (var cur in m_right)
                {
                    yield return cur;
                }
            }
        }

        #endregion

        #region Static Empty

        private static readonly EmptyCollection s_empty = new EmptyCollection();
        public static ImmutableCollection<T> Empty
        {
            get { return s_empty; }
        }

        #endregion

        public abstract bool IsEmpty { get; }
        public abstract int Count { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public abstract T Front { get; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public abstract T Back { get; }

        internal ImmutableCollection()
        {
        }

        public abstract ImmutableCollection<T> AddFront(T value);
        public abstract ImmutableCollection<T> AddBack(T value);
        public abstract ImmutableCollection<T> RemoveFront();
        public abstract ImmutableCollection<T> RemoveBack();
        public abstract IEnumerator<T> GetEnumerator();

        public bool Contains(T value)
        {
            var comp = EqualityComparer<T>.Default;
            foreach (var cur in this)
            {
                if (comp.Equals(value, cur))
                {
                    return true;
                }
            }

            return false;
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IPersistentCollection<T> Members

        IPersistentCollection<T> IPersistentCollection<T>.Add(T value)
        {
            return AddBack(value);
        }

        IPersistentCollection<T> IPersistentCollection<T>.Remove(T value)
        {
            var list = new List<T>();
            var comp = EqualityComparer<T>.Default;
            bool found = false;
            foreach (var cur in this)
            {
                if (!found && comp.Equals(value, cur))
                {
                    found = true;
                }
                else
                {
                    list.Add(cur);
                }
            }

            return ImmutableCollection.Create(list);
        }

        public IPersistentCollection<T> Clear()
        {
            return ImmutableCollection<T>.Empty;
        }

        #endregion
    }

    public static class ImmutableCollection
    {
        public static ImmutableCollection<T> Create<T>(IEnumerable<T> enumerable)
        {
            var col = ImmutableCollection<T>.Empty;
            foreach (var cur in enumerable)
            {
                col = col.AddBack(cur);
            }

            return col;
        }

        public static ImmutableCollection<T> CreateFromArguments<T>(params T[] array)
        {
            return Create(array);
        }

    }
}
