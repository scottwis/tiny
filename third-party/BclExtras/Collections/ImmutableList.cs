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
    public sealed class ImmutableList<T> : IEnumerable<T>, IPersistentList<T>, IPersistentCollection<T>
    {
        #region Static Members

        private static readonly ImmutableList<T> s_empty = new ImmutableList<T>();

        public static ImmutableList<T> Empty
        {
            get { return s_empty; }
        }

        #endregion

        private readonly ImmutableArray<T> m_array;

        public int Count
        {
            get { return m_array.Count; }
        }

        public bool IsEmpty
        {
            get { return m_array.IsEmpty; }
        }

        public T this[int index]
        {
            get { return m_array[index]; }
        }

        private ImmutableList()
        {
            m_array = ImmutableArray<T>.Empty;
        }

        public ImmutableList(T[] data)
        {
            m_array = new ImmutableArray<T>(data);
        }

        public ImmutableList(IEnumerable<T> data)
        {
            m_array = new ImmutableArray<T>(data);
        }

        public ImmutableList(ICollection<T> col)
        {
            m_array = new ImmutableArray<T>(col);
        }

        public ImmutableList(IReadOnlyCollection<T> col)
        {
            m_array = new ImmutableArray<T>(col);
        }

        public ImmutableList(ImmutableArray<T> data)
        {
            m_array = data;
        }

        public ImmutableList<T> Add(T value)
        {
            return new ImmutableList<T>(m_array.Concat(value));
        }

        public ImmutableList<T> AddRange(IEnumerable<T> data)
        {
            return new ImmutableList<T>(m_array.Concat(data));
        }

        public ImmutableList<T> Insert(int index, T value)
        {
            T[] data = new T[Count + 1];
            for (int i = 0; i < index; i++)
            {
                data[i] = m_array[i];
            }
            data[index] = value;
            for (int i = index; i < Count; i++)
            {
                data[i + 1] = m_array[i];
            }
            return new ImmutableList<T>(data);
        }

        public ImmutableList<T> Remove(T value)
        {
            var index = IndexOf(value);
            if (index < 0)
            {
                return this;
            }

            return RemoveAt(index);
        }

        public ImmutableList<T> RemoveAll(Func<T, bool> predicate)
        {
            return new ImmutableList<T>(this.Where(x => !predicate(x)));
        }

        public ImmutableList<T> RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            int current = 0;
            var array = new ImmutableArray<T>(
                m_array.Where((x) => current++ != index),
                Count - 1);
            return new ImmutableList<T>(array);
        }

        public ImmutableList<T> Clear()
        {
            return new ImmutableList<T>();
        }

        public int IndexOf(T value)
        {
            var comp = EqualityComparer<T>.Default;
            for (int i = 0; i < m_array.Count; ++i)
            {
                if (comp.Equals(value, m_array[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Contains(T value)
        {
            return m_array.Contains(value);
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            if (m_array == null)
            {
                yield break;
            }

            for (int i = 0; i < m_array.Count; ++i)
            {
                yield return m_array[i];
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IPersistentList<T> Members

        IPersistentList<T> IPersistentList<T>.Insert(int index, T value)
        {
            return Insert(index, value);
        }

        IPersistentList<T> IPersistentList<T>.RemoveAt(int index)
        {
            return RemoveAt(index);
        }

        IPersistentList<T> IPersistentList<T>.Clear()
        {
            return Clear();
        }

        IPersistentList<T> IPersistentList<T>.Add(T value)
        {
            return Add(value);
        }

        IPersistentList<T> IPersistentList<T>.Remove(T value)
        {
            return Remove(value);
        }

        #endregion

        #region IPersistentCollection<T> Members

        IPersistentCollection<T> IPersistentCollection<T>.Add(T value)
        {
            return Add(value);
        }

        IPersistentCollection<T> IPersistentCollection<T>.Remove(T value)
        {
            return Remove(value);
        }

        IPersistentCollection<T> IPersistentCollection<T>.Clear()
        {
            return Clear();
        }

        #endregion
    }

    public static class ImmutableList
    {
        public static ImmutableList<T> Create<T>(IEnumerable<T> enumerable)
        {
            return new ImmutableList<T>(enumerable);
        }

        public static ImmutableList<T> Create<T>(ICollection<T> col)
        {
            return new ImmutableList<T>(col);
        }

        public static ImmutableList<T> Create<T>(IReadOnlyCollection<T> col)
        {
            return new ImmutableList<T>(col);
        }

        public static ImmutableList<T> CreateFromArguments<T>(params T[] args)
        {
            return new ImmutableList<T>(args);
        }
    }
}
