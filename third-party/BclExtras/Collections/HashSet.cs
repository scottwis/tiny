using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Collections;
using System.Diagnostics;
#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Collections
{

#if !NETFX_35

    /// <summary>
    /// TODO: Implement a non-dictionary based implementation for Hashset.  For now going with
    /// Dictionary implementation in order to maintain compatibility with the 3.5 version.  
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
    public class HashSet<T> : ICollection<T>
    {
        private Dictionary<T, object> m_map = new Dictionary<T, object>();

        public HashSet()
        {
            m_map = new Dictionary<T, object>();
        }

        public HashSet(IEqualityComparer<T> comparer)
        {
            m_map = new Dictionary<T, object>(comparer);
        }

        public bool Add(T item)
        {
            if (m_map.ContainsKey(item))
            {
                return false;
            }

            m_map.Add(item, null);
            return true;
        }

        public bool Remove(T item)
        {
            return m_map.Remove(item);
        }

        public bool Contains(T item)
        {
            return m_map.ContainsKey(item);
        }

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        public void Clear()
        {
            m_map.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return m_map.Count; } 
        }

        public bool IsReadOnly
        {
            get { return false; } 
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return m_map.Keys.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }

#endif

    public static class HashSet
    {
        public static HashSet<T> Create<T>()
            where T: IComparable<T>
        {
            return new HashSet<T>(EqualityComparer<T>.Default);
        }

        public static HashSet<T> Create<T>(IEqualityComparer<T> comp)
        {
            return new HashSet<T>(comp);
        }
    }
}