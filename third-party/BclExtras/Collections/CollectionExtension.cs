using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras.Collections
{
    public static class CollectionExtension
    {
        #region ICollection<T>
        /// <summary>
        /// Lots of interfaces don't implement AddRange but are a Collection(Of T).  This makes
        /// it easy to AddRange them
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="col"></param>
        /// <param name="enumerable"></param>
        public static void AddRange<T>(this ICollection<T> col, IEnumerable<T> enumerable)
        {
            foreach (var cur in enumerable)
            {
                col.Add(cur);
            }
        }

        #endregion

        #region Dictionary<TKey,TValue>

        public static Option<TValue> TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue value;
            if (dict.TryGetValue(key, out value))
            {
                return Option.Create(value);
            }

            return Option.Empty;
        }

        #endregion

    }
}
