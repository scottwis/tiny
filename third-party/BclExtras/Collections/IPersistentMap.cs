using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras.Collections
{
    /// <summary>
    /// Interface for a persistent map
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IPersistentMap<TKey,TValue>  : IReadOnlyMap<TKey,TValue>
    {
        IPersistentMap<TKey, TValue> Add(TKey key, TValue value);
        IPersistentMap<TKey, TValue> Update(TKey key, TValue value);
        IPersistentMap<TKey, TValue> Remove(TKey key);
    }
}
