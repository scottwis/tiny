using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras.Collections
{
    /// <summary>
    /// Basic IReadOnlyMap semantics
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IReadOnlyMap<TKey,TValue> : IEnumerable<Tuple<TKey,TValue>>
    {
        IEnumerable<TKey> Keys { get; }
        IEnumerable<TValue> Values { get; }
        TValue this[TKey key] { get; }
        int Count { get; }

        bool ContainsKey(TKey key);
        Option<TValue> Find(TKey key);
    }
}
