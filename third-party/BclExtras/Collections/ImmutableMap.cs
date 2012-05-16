using System;
using System.Collections.Generic;
using System.Text;
using BclExtras.Threading;
using System.Diagnostics;
#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Collections
{
    [Immutable]
    [Serializable]
    [DebuggerTypeProxy(typeof(DebuggerEnumerableView<>))]
    public sealed class ImmutableMap<TKey, TValue> : IPersistentMap<TKey, TValue>
    {
        private readonly ImmutableAvlTree<ComparerNode<TKey>, TValue> m_tree;
        private readonly IComparer<TKey> m_comparer;

        public IEnumerable<TKey> Keys
        {
            get { return m_tree.Keys.Select((x) => x.Value); }
        }

        public IEnumerable<TValue> Values
        {
            get { return m_tree.Values; }
        }

        public IComparer<TKey> Comparer
        {
            get { return m_comparer; }
        }

        public bool IsEmpty
        {
            get { return m_tree.IsEmpty; }
        }

        public int Count
        {
            get { return m_tree.Count; }
        }

        public TValue this[TKey key]
        {
            get { return m_tree.Find(ComparerNode.Create(m_comparer, key)).Value; }
        }

        public ImmutableMap(IComparer<TKey> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            m_comparer = comparer;
            m_tree = ImmutableAvlTree<ComparerNode<TKey>, TValue>.Empty;
        }

        private ImmutableMap(IComparer<TKey> comparer, ImmutableAvlTree<ComparerNode<TKey>, TValue> tree)
        {
            m_comparer = comparer;
            m_tree = tree;
        }

        public ImmutableMap<TKey, TValue> Add(TKey key, TValue value)
        {
            return new ImmutableMap<TKey, TValue>(
                m_comparer,
                m_tree.Add(ComparerNode.Create(m_comparer, key), value));
        }

        public ImmutableMap<TKey, TValue> Update(TKey key, TValue value)
        {
            return new ImmutableMap<TKey, TValue>( m_comparer, m_tree.Update(ComparerNode.Create(m_comparer,key), value));
        }

        public ImmutableMap<TKey, TValue> Delete(TKey key)
        {
            return new ImmutableMap<TKey, TValue>( m_comparer, m_tree.Delete(ComparerNode.Create(m_comparer, key)));
        }

        public Option<TValue> Find(TKey key)
        {
            TValue value;
            if (TryGetValue(key, out value))
            {
                return Option.Create(value);
            }

            return Option.Empty;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_tree.TryFind(ComparerNode.Create(m_comparer, key), out value);
        }

        public bool ContainsKey(TKey key)
        {
            TValue value;
            return m_tree.TryFind(ComparerNode.Create(m_comparer,key), out value);
        }

        #region IEnumerable<Tuple<TKey,TValue>> Members

        IEnumerator<Tuple<TKey, TValue>> IEnumerable<Tuple<TKey, TValue>>.GetEnumerator()
        {
            return m_tree.Select((x) => Tuple.Create(x.First.Value, x.Second)).GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Tuple<TKey,TValue>>)this).GetEnumerator();
        }

        #endregion

        #region IPersistentMap<TKey,TValue> Members

        IPersistentMap<TKey, TValue> IPersistentMap<TKey, TValue>.Add(TKey key, TValue value)
        {
            return Add(key, value);
        }

        IPersistentMap<TKey, TValue> IPersistentMap<TKey, TValue>.Update(TKey key, TValue value)
        {
            return Update(key, value);
        }

        IPersistentMap<TKey, TValue> IPersistentMap<TKey, TValue>.Remove(TKey key)
        {
            return Delete(key);
        }

        #endregion

    }

    public static class ImmutableMap
    {
        public static ImmutableMap<TKey, TValue> Create<TKey, TValue>()
            where TKey : IComparable<TKey>
        {
            return new ImmutableMap<TKey, TValue>(Comparer<TKey>.Default);
        }

        public static ImmutableMap<TKey, TValue> Create<TKey, TValue>(IEnumerable<Tuple<TKey,TValue>> enumerable)
            where TKey : IComparable<TKey>
        {
            return Create(Comparer<TKey>.Default, enumerable);
        }

        public static ImmutableMap<TKey, TValue> Create<TKey, TValue>(IComparer<TKey> comp)
        {
            return new ImmutableMap<TKey, TValue>(comp);
        }

        public static ImmutableMap<TKey, TValue> Create<TKey, TValue>(IComparer<TKey> comp, IEnumerable<Tuple<TKey,TValue>> enumerable)
        {
            var map = new ImmutableMap<TKey, TValue>(comp);
            foreach (var t in enumerable)
            {
                map = map.Add(t.First, t.Second);
            }
            return map;
        }

        public static ImmutableMap<TKey, TValue> Create<TKey, TValue>(IComparer<TKey> comp, IDictionary<TKey, TValue> source)
        {
            var map = new ImmutableMap<TKey, TValue>(comp);
            foreach (var t in source)
            {
                map = map.Add(t.Key, t.Value);
            }
            return map;
        }

    }
}
