using System;
using System.Collections.Generic;
using BclExtras.Collections;

#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Threading
{
    public sealed class WeakDictionary<TKey, TValue>
    {
        private Dictionary<TKey, WeakReference> m_map;
        private int m_threshold = 70;

        public List<Tuple<TKey, TValue>> Pairs
        {
            get 
            { 
                return m_map
                        .Select(p => Tuple.Create(p.Key, p.Value.Target))
                        .Where(t => t.Second != null)
                        .Select(t => Tuple.Create(t.First, (TValue)t.Second))
                        .ToList(); 
            }
        }

        public List<TValue> Values
        {
            get { return Pairs.Select(x => x.Second).ToList(); }
        }

        /// <summary>
        /// A percentage value between 0 and 100 inclusive.  When the percentage of Values collected is greater than
        /// or equal to this percentage then a collection will occur and the underlying table structure
        /// will be shrunk to only valid values
        /// </summary>
        public int Threshold
        {
            get { return m_threshold; }
            set
            {
                if (value < 0 || value > 100)
                {
                    throw new ArgumentOutOfRangeException("Threshold must be >= 0 and <= 100");
                }

                m_threshold = value;
            }
        }

        public WeakDictionary()
            : this(EqualityComparer<TKey>.Default)
        {
        }

        public WeakDictionary(IEqualityComparer<TKey> comparer)
        {
            m_map = new Dictionary<TKey, WeakReference>(comparer);
        }

        public void Add(TKey key, TValue value)
        {
            MaybeCompact();
            m_map.Add(key, new WeakReference(value));
        }

        public void Put(TKey key, TValue value)
        {
            MaybeCompact();
            m_map[key] = new WeakReference(value);
        }

        public bool Remove(TKey key)
        {
            MaybeCompact();
            return m_map.Remove(key);
        }

        public Option<TValue> TryGetValue(TKey key)
        {
            WeakReference weakRef;
            if (!m_map.TryGetValue(key, out weakRef))
            {
                return Option.Empty;
            }

            var target = weakRef.Target;
            if (target == null)
            {
                return Option.Empty;
            }

            return (TValue)target;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var option = TryGetValue(key);
            value = option.ValueOrDefault;
            return option.HasValue;
        }

        private void MaybeCompact()
        {
            if (ShouldCompact())
            {
                Compact();
            }
        }

        public void Compact()
        {
            var old = m_map;
            m_map = new Dictionary<TKey, WeakReference>(old.Comparer);
            foreach (var cur in old.Where(p => p.Value.IsAlive))
            {
                m_map.Add(cur.Key, cur.Value);
            }
        }

        private bool ShouldCompact()
        {
            var countCollected = (double)(m_map.Values.Where(x => !x.IsAlive).Count());
            var diff = countCollected / (double)(m_map.Count);
            var percent = (int)(100 * diff);
            return percent >= m_threshold;
        }
    }
}
