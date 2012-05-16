using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Collections
{
    #region ListExtension

    public enum DiffUpdateResult
    {
        Added,
        Removed,
        Unchanged
    }

    public delegate bool EqualityCallback<T>(T left, T right);

    /// <summary>
    /// When converted to 3.0 this will be an extension method class
    /// </summary>
    public static class ListExtension
    {
        #region DiffUpdate


        private class ListUtilEqualityComparer<T> : IEqualityComparer<T>
        {
            private EqualityCallback<T> m_eqCb;

            public ListUtilEqualityComparer(EqualityCallback<T> cb)
            {
                m_eqCb = cb;
            }

            #region IEqualityComparer<T> Members

            public bool Equals(T x, T y)
            {
                return m_eqCb(x, y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }

        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static IEnumerable<Tuple<T, DiffUpdateResult>> DiffUpdate<T>(IList<T> list, IEnumerable<T> updateAgainst)
        {
            return DiffUpdate(list, updateAgainst, EqualityComparer<T>.Default);
        }

        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static IEnumerable<Tuple<T, DiffUpdateResult>> DiffUpdate<T>(IList<T> list, IEnumerable<T> updateAgainst, EqualityCallback<T> comparer)
        {
            Contract.ThrowIfNull(comparer);
            return DiffUpdate(
                list,
                updateAgainst,
                new ListUtilEqualityComparer<T>(comparer));
        }

        /// <summary>
        /// Updates an existing list against a list of new items.  Will return the list
        /// of items removed.  
        /// </summary>
        /// <param name="list"></param>
        /// <param name="updateAgainst"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static IEnumerable<Tuple<T, DiffUpdateResult>> DiffUpdate<T>(IList<T> list, IEnumerable<T> updateAgainst, IEqualityComparer<T> comparer)
        {
            Contract.ThrowIfNull(list);
            Contract.ThrowIfNull(updateAgainst);
            Contract.ThrowIfNull(comparer);

            const int InOld = 0x0001;
            const int InNew = 0x0002;

            // Add all of the items
            Dictionary<T, int> map = new Dictionary<T, int>(comparer);
            foreach (T cur in list)
            {
                map[cur] = InOld;
            }

            foreach (T cur in updateAgainst)
            {
                int flags;
                if (!map.TryGetValue(cur, out flags))
                {
                    flags = 0;
                }

                map[cur] = flags | InNew;
            }

            List<Tuple<T, DiffUpdateResult>> result = new List<Tuple<T, DiffUpdateResult>>();
            foreach (KeyValuePair<T, int> pair in map)
            {
                if (InOld == pair.Value)
                {
                    // Only in the old
                    list.Remove(pair.Key);
                    result.Add(Tuple.Create(pair.Key, DiffUpdateResult.Removed));
                }
                else if (InNew == pair.Value)
                {
                    // Only in the new
                    list.Add(pair.Key);
                    result.Add(Tuple.Create(pair.Key, DiffUpdateResult.Added));
                }
                else
                {
                    Contract.ThrowIfFalse(pair.Value == (InOld | InNew));
                    result.Add(Tuple.Create(pair.Key, DiffUpdateResult.Unchanged));
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// A more flexible add range method
        /// </summary>
        /// <typeparam name="TBase"></typeparam>
        /// <typeparam name="TDerived"></typeparam>
        /// <param name="list"></param>
        /// <param name="enumerable"></param>
        public static void AddRange<TBase, TDerived>(this ICollection<TBase> list, IEnumerable<TDerived> enumerable)
            where TDerived : TBase
        {

            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            foreach (TDerived u in enumerable)
            {
                list.Add(u);
            }
        }

        public static void AddRange<TItem, TElement>(this ICollection<TItem> list, IEnumerable<TElement> enumerable, Func<TElement, TItem> del)
        {
            foreach (var cur in enumerable)
            {
                list.Add(del(cur));
            }
        }

        /// <summary>
        /// Remove all instances of U from the passed in list
        /// </summary>
        /// <typeparam name="TBase"></typeparam>
        /// <typeparam name="TDerived"></typeparam>
        /// <param name="list"></param>
        public static void RemoveAllDerived<TBase, TDerived>(this IList<TBase> list)
            where TDerived : class, TBase
            where TBase : class
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            int i = 0;
            while (i < list.Count)
            {
                TBase cur = list[i];
                if (cur != null && cur is TDerived)
                {
                    list.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }

        /// <summary>
        /// Remove all instances of U from the passed in list that meets the specified delegate
        /// </summary>
        /// <typeparam name="TBase"></typeparam>
        /// <typeparam name="TDerived"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        public static void RemoveAll<TBase, TDerived>(this IList<TBase> list, Predicate<TDerived> predicate)
            where TDerived : class, TBase
            where TBase : class
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            int i = 0;
            while (i < list.Count)
            {
                TDerived cur = list[i] as TDerived;
                if (cur != null && predicate(cur))
                {
                    list.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }
    }

    #endregion

    // TODO:  Add an extension for BindingList<T> which does a DiffUpdate but does all of the 
    // smarts to turn off change events and such whhile the list is updating.  Then turns them
    // back on once the update is done raises a ResetBinding if the list changed
}
