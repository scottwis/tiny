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
    [Immutable]
    [Serializable]
    [SuppressMessage("Microsoft.Naming", "CA1710")]
    [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
    public sealed class ImmutableArray<T> : IReadOnlyList<T>, IEnumerable<T>
    {
        #region Static Helpers

        private static readonly ImmutableArray<T> s_empty = new ImmutableArray<T>();

        public static ImmutableArray<T> Empty
        {
            get { return s_empty; }
        }

        #endregion

        private readonly T[] m_array;

        public int Count
        {
            get { return m_array != null ? m_array.Length : 0; }
        }

        public bool IsEmpty
        {
            get { return 0 == Count; }
        }

        public T this[int index]
        {
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Empty List");
                }

                return m_array[index];
            }
        }

        private ImmutableArray()
        {
        }

        public ImmutableArray(IEnumerable<T> data)
        {
            m_array = data.ToArray();
        }

        public ImmutableArray(ICollection<T> col)
            : this(col, col.Count)
        {

        }

        public ImmutableArray(IReadOnlyCollection<T> col)
            : this(col, col.Count)
        {
        }

        /// <summary>
        /// Sightly optimized version where the count of the data is known before hand
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public ImmutableArray(IEnumerable<T> data, int count)
        {
            m_array = new T[count];
            int index = 0;
            foreach (var item in data)
            {
                m_array[index] = item;
                ++index;
            }

            if (index != count)
            {
                throw new ArgumentException("Count and size of collection do not match");
            }
        }

        public int IndexOf(T value)
        {
            if (IsEmpty)
            {
                return -1;
            }

            return Array.IndexOf(m_array, value);
        }

        public bool Contains(T value)
        {
            return IndexOf(value) >= 0;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            if (m_array == null)
            {
                yield break;
            }

            for (int i = 0; i < m_array.Length; ++i)
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
    }

    public static class ImmutableArray
    {
        public static ImmutableArray<T> Create<T>(IEnumerable<T> enumerable)
        {
            return new ImmutableArray<T>(enumerable);
        }

        public static ImmutableArray<T> Create<T>(ICollection<T> col)
        {
            return new ImmutableArray<T>(col);
        }

        public static ImmutableArray<T> Create<T>(IReadOnlyCollection<T> col)
        {
            return new ImmutableArray<T>(col);
        }

        public static ImmutableArray<T> CreateFromArguments<T>(params T[] array)
        {
            return Create(array);
        }
    }
}
