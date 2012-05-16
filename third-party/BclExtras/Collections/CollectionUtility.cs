using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using BclExtras.Threading;
#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Collections
{
    public static class CollectionUtility
    {
        #region CollectionShim

        [Immutable]
        [Serializable]
        private class CollectionShim<T> : ICollection<T>
        {
            private readonly IReadOnlyCollection<T> m_col;

            internal CollectionShim(IReadOnlyCollection<T> col)
            {
                m_col = col;
            }

            #region ICollection<T> Members

            public void Add(T item)
            {
                throw CreateNotSupportedException();
            }

            public void Clear()
            {
                throw CreateNotSupportedException();
            }

            public bool Contains(T item)
            {
                return m_col.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                m_col.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return m_col.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(T item)
            {
                throw CreateNotSupportedException();
            }

            #endregion

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return m_col.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.IEnumerable)m_col).GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region ListShim

        [Immutable]
        [Serializable]
        private sealed class ListShim<T> : CollectionShim<T>, IList<T>
        {
            private readonly IReadOnlyList<T> m_list;

            internal ListShim(IReadOnlyList<T> list)
                : base(list)
            {
                m_list = list;
            }

            #region IList<T> Members

            public int IndexOf(T item)
            {
                return m_list.IndexOf(item);
            }

            public void Insert(int index, T item)
            {
                throw CreateNotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw CreateNotSupportedException();
            }

            public T this[int index]
            {
                get
                {
                    return m_list[index];
                }
                set
                {
                    throw CreateNotSupportedException();
                }
            }

            #endregion
        }

        #endregion

        #region DictionaryShim

        [Immutable]
        [Serializable]
        private class DictionaryShim<TKey, TValue> : IDictionary<TKey, TValue>
        {
            private readonly IReadOnlyMap<TKey, TValue> m_map;

            internal DictionaryShim(IReadOnlyMap<TKey, TValue> map)
            {
                m_map = map;
            }

            #region IDictionary<TKey,TValue> Members

            public void Add(TKey key, TValue value)
            {
                throw CreateNotSupportedException();
            }

            public bool ContainsKey(TKey key)
            {
                return m_map.ContainsKey(key);
            }

            public ICollection<TKey> Keys
            {
                get { return new List<TKey>(m_map.Keys); }
            }

            public bool Remove(TKey key)
            {
                throw CreateNotSupportedException();
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                var result = m_map.Find(key);
                value = result.HasValue
                    ? result.Value
                    : default(TValue);
                return result.HasValue;
            }

            public ICollection<TValue> Values
            {
                get { return new List<TValue>(m_map.Values); }
            }

            public TValue this[TKey key]
            {
                get
                {
                    return m_map[key];
                }
                set
                {
                    throw CreateNotSupportedException();
                }
            }

            #endregion

            #region ICollection<KeyValuePair<TKey,TValue>> Members

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                throw CreateNotSupportedException();
            }

            public void Clear()
            {
                throw CreateNotSupportedException();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                TValue value;
                if (TryGetValue(item.Key, out value))
                {
                    return EqualityComparer<TValue>.Default.Equals(value, item.Value);
                }

                return false;
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                GetKeyValuePairs().CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return m_map.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                throw CreateNotSupportedException();
            }

            #endregion

            #region IEnumerable<KeyValuePair<TKey,TValue>> Members

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return GetKeyValuePairs().GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetKeyValuePairs().GetEnumerator();
            }

            #endregion

            private IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairs()
            {
                return m_map.Select(x => new KeyValuePair<TKey, TValue>(x.First, x.Second));
            }
        }

        #endregion

        #region ObjectCollectionShim

        [Immutable]
        [Serializable]
        private class ObjectCollectionShim<T> : System.Collections.ICollection
        {
            private readonly IReadOnlyCollection<T> m_col;

            internal ObjectCollectionShim(IReadOnlyCollection<T> col)
            {
                m_col = col;
            }

            #region ICollection Members

            public void CopyTo(Array array, int index)
            {
                foreach (var item in m_col)
                {
                    array.SetValue(item, index);
                    ++index;
                }
            }

            public int Count
            {
                get { return m_col.Count; }
            }

            public bool IsSynchronized
            {
                get { return true; }
            }

            public object SyncRoot
            {
                get { return m_col; }
            }

            #endregion

            #region IEnumerable Members

            public IEnumerator GetEnumerator()
            {
                return m_col.GetEnumerator();
            }

            #endregion
        }

        #endregion

        #region ObjectListShim

        [Immutable]
        [Serializable]
        private sealed class ObjectListShim<T> : ObjectCollectionShim<T>, System.Collections.IList
        {
            private readonly IReadOnlyList<T> m_list;

            internal ObjectListShim(IReadOnlyList<T> list)
                : base(list)
            {
                m_list = list;
            }

            #region IList Members

            public int Add(object value)
            {
                throw CreateNotSupportedException();
            }

            public void Clear()
            {
                throw CreateNotSupportedException();
            }

            public bool Contains(object value)
            {
                return m_list.Contains((T)value);
            }

            public int IndexOf(object value)
            {
                return m_list.IndexOf((T)value);
            }

            public void Insert(int index, object value)
            {
                throw CreateNotSupportedException();
            }

            public bool IsFixedSize
            {
                get { return true; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public void Remove(object value)
            {
                throw CreateNotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw CreateNotSupportedException();
            }

            public object this[int index]
            {
                get
                {
                    return m_list[index];
                }
                set
                {
                    throw CreateNotSupportedException();
                }
            }

            #endregion
        }


        #endregion

        #region ObjectDictionaryShim

        [Immutable]
        [Serializable]
        private sealed class ObjectDictionaryShim<TKey, TValue> : IDictionary
        {
            #region Enumerator

            private sealed class Enumerator : IDictionaryEnumerator
            {
                private IEnumerator<Tuple<TKey, TValue>> m_enumerator;

                internal Enumerator(IEnumerator<Tuple<TKey, TValue>> enumerator)
                {
                    m_enumerator = enumerator;
                }

                #region IDictionaryEnumerator Members

                public DictionaryEntry Entry
                {
                    get { return new DictionaryEntry(Key, Value); }
                }

                public object Key
                {
                    get { return m_enumerator.Current.First; }
                }

                public object Value
                {
                    get { return m_enumerator.Current.Second; }
                }

                #endregion

                #region IEnumerator Members

                public object Current
                {
                    get { return Entry; }
                }

                public bool MoveNext()
                {
                    return m_enumerator.MoveNext();
                }

                public void Reset()
                {
                    m_enumerator.Reset();
                }

                #endregion
            }

            #endregion

            private readonly IReadOnlyMap<TKey, TValue> m_map;

            internal ObjectDictionaryShim(IReadOnlyMap<TKey, TValue> map)
            {
                m_map = map;
            }

            #region IDictionary Members

            public void Add(object key, object value)
            {
                throw CreateNotSupportedException();
            }

            public void Clear()
            {
                throw CreateNotSupportedException();
            }

            public bool Contains(object key)
            {
                return m_map.ContainsKey((TKey)key);
            }

            public IDictionaryEnumerator GetEnumerator()
            {
                return new Enumerator(m_map.GetEnumerator());
            }

            public bool IsFixedSize
            {
                get { return true; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public ICollection Keys
            {
                get { return CreateObjectICollection(ImmutableArray.Create(m_map.Keys)); }
            }

            public void Remove(object key)
            {
                throw CreateNotSupportedException();
            }

            public ICollection Values
            {
                get { return CreateObjectICollection(ImmutableArray.Create(m_map.Values)); }
            }

            public object this[object key]
            {
                get { return m_map.Find((TKey)key).ValueOrDefault; }
                set
                {
                    throw CreateNotSupportedException();
                }
            }

            #endregion

            #region ICollection Members

            public void CopyTo(Array array, int index)
            {
                foreach (var cur in m_map)
                {
                    array.SetValue(cur, index);
                    index++;
                }
            }

            public int Count
            {
                get { return m_map.Count; }
            }

            public bool IsSynchronized
            {
                get { return true; }
            }

            public object SyncRoot
            {
                get { return m_map; }
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(m_map.GetEnumerator());
            }

            #endregion
        }


        #endregion

        public static IEnumerable<T> CreateEmptyEnumerable<T>()
        {
            yield break;
        }

        public static IEnumerable<T> CreateEnumerable<T>(T value)
        {
            yield return value;
        }

        /// <summary>
        /// Useful when interopting IReadOnlyCollection`1 into a scenario where
        /// an instance of ICollection`1 is needed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col"></param>
        /// <returns></returns>
        public static ICollection<T> CreateICollection<T>(IReadOnlyCollection<T> col)
        {
            return new CollectionShim<T>(col);
        }

        /// <summary>
        /// Useful when interopting IReadOnlyList`1 into a scenario where an instance
        /// of IList`1 is needed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<T> CreateIList<T>(IReadOnlyList<T> list)
        {
            return new ListShim<T>(list);
        }

        public static IDictionary<TKey, TValue> CreateIDictionary<TKey, TValue>(IReadOnlyMap<TKey, TValue> map)
        {
            return new DictionaryShim<TKey, TValue>(map);
        }

        public static System.Collections.ICollection CreateObjectICollection<T>(IReadOnlyCollection<T> col)
        {
            return new ObjectCollectionShim<T>(col);
        }

        public static System.Collections.IList CreateObjectIList<T>(IReadOnlyList<T> list)
        {
            return new ObjectListShim<T>(list);
        }

        public static System.Collections.IDictionary CreateObjectIDictionary<TKey, TValue>(IReadOnlyMap<TKey, TValue> map)
        {
            return new ObjectDictionaryShim<TKey, TValue>(map);
        }

        public static IEnumerable<int> GetRangeCount(int start, int count)
        {
            for (int i = start; i < start + count; ++i)
            {
                yield return i;
            }
        }

        private static Exception CreateNotSupportedException()
        {
            return new NotSupportedException("Collection is read only");
        }

    }
}
