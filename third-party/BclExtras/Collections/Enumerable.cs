using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Collections
{
#if !NETFX_35

    /// <summary>
    /// Helpful extension methods for objects of type IEnumerable`1
    /// </summary>
    public static partial class Enumerable
    {
        #region Query Operations

        #region Where

        public static IEnumerable<T> Where<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            foreach (var cur in enumerable)
            {
                if (predicate(cur))
                {
                    yield return cur;
                }
            }
        }

        #endregion

        #region First

        public static T First<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            using (var e = enumerable.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    throw EnumerableExtra.CreateEmpty();
                }

                return e.Current;
            }
        }

        public static T First<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }


            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            foreach (var cur in enumerable)
            {
                if (predicate(cur))
                {
                    return cur;
                }
            }

            throw EnumerableExtra.CreateNoMatch();
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            using (var e = enumerable.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    return default(T);
                }

                return e.Current;
            }
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {

            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            foreach (var cur in enumerable)
            {
                if (predicate(cur))
                {
                    return cur;
                }
            }

            return default(T);
        }

        #endregion

        #region Any

        /// <summary>
        /// Any items in the enumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static bool Any<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            using (var e = enumerable.GetEnumerator())
            {
                return e.MoveNext();
            }
        }

        /// <summary>
        /// Any items which meet the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Any<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            foreach (var cur in enumerable)
            {
                if (predicate(cur))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Select

        public static IEnumerable<TReturn> Select<TReturn, T>(this IEnumerable<T> enumerable, Func<T, TReturn> del)
        {
            foreach (var cur in enumerable)
            {
                yield return del(cur);
            }
        }

        #endregion

        #region OfType

        public static IEnumerable<T> OfType<T>(this System.Collections.IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            foreach (var cur in enumerable)
            {
                if (cur is T)
                {
                    yield return (T)cur;
                }
            }
        }

        #endregion

        #region Contains

        /// <summary>
        /// Whether or not this structure contains the specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> enumerable, T value)
        {
            return Contains(enumerable, value, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Whether or not this structure contains this value base on the equality comparer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool Contains<T>(this IEnumerable<T> enumerable, T value, IEqualityComparer<T> comparer)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            return ContainsHelper(enumerable, value, comparer);
        }

        private static bool ContainsHelper<T>(this IEnumerable<T> enumerable, T value, IEqualityComparer<T> comparer)
        {
            foreach (var cur in enumerable)
            {
                if (comparer.Equals(cur, value))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region All

        /// <summary>
        /// Returns true if the predicate is true for all items in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool All<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }


            return AllHelper(enumerable, predicate);
        }

        private static bool AllHelper<T>(IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            foreach (var cur in enumerable)
            {
                if (!predicate(cur))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Skip

        public static IEnumerable<T> Skip<T>(this IEnumerable<T> enumerable, int count)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            return SkipHelper(enumerable, count);
        }

        private static IEnumerable<T> SkipHelper<T>(IEnumerable<T> enumerable, int count)
        {
            using (var e = enumerable.GetEnumerator())
            {
                while (count > 0 && e.MoveNext())
                {
                    count--;
                }

                while (e.MoveNext())
                {
                    yield return e.Current;
                }
            }
        }

        #endregion

        #region SkipWhile

        public static IEnumerable<T> SkipWhile<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return SkipWhileHelper(enumerable, predicate);
        }

        private static IEnumerable<T> SkipWhileHelper<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            using (var e = enumerable.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    if (!predicate(e.Current))
                    {
                        yield return e.Current;
                        break;
                    }
                }

                while (e.MoveNext())
                {
                    yield return e.Current;
                }
            }
        }

        #endregion

        #region Take

        /// <summary>
        /// Takes count elemets from the list.  If count is greater than the number of elements in the list
        /// then only the elements in the list will be returned
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> Take<T>(this IEnumerable<T> enumerable, int count)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            return TakeHelper(enumerable, count);
        }

        private static IEnumerable<T> TakeHelper<T>(IEnumerable<T> enumerable, int count)
        {
            using (var e = enumerable.GetEnumerator())
            {
                while (count > 0 && e.MoveNext())
                {
                    yield return e.Current;
                    count--;
                }
            }
        }

        #endregion

        #region TakeWhile

        public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return TakeWhileHelper(enumerable, predicate);
        }

        private static IEnumerable<T> TakeWhileHelper<T>(IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            foreach (var cur in enumerable)
            {
                if (!predicate(cur))
                {
                    break;
                }

                yield return cur;
            }
        }

        #endregion

        #region SelectMany

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, IEnumerable<TResult>> func)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            return SelectManyHelper(enumerable, func);
        }

        private static IEnumerable<TResult> SelectManyHelper<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, IEnumerable<TResult>> func)
        {
            foreach (var cur in enumerable)
            {
                var innerEnumerable = func(cur);
                foreach (var inner in innerEnumerable)
                {
                    yield return inner;
                }
            }
        }

        #endregion

        #endregion

        #region Operations on All

        #endregion

        #region List Modifiers

        #region Concat

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, IEnumerable<T> enumerableToAdd)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            return ConcatHelper(enumerable, enumerableToAdd);
        }

        private static IEnumerable<T> ConcatHelper<T>(this IEnumerable<T> enumerable, IEnumerable<T> enumerableToAdd)
        {
            foreach (var cur in enumerable)
            {
                yield return cur;
            }

            foreach (var cur in enumerableToAdd)
            {
                yield return cur;
            }
        }

        #endregion

        #region Distinct

        /// <summary>
        /// Returns the distinct elements in the enumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> enumerable)
        {
            return Distinct(enumerable, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Returns the distinct elements in the enumerable based off of the equality parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            return DistinctHelper(enumerable, comparer);
        }

        private static IEnumerable<T> DistinctHelper<T>(IEnumerable<T> enumerable, IEqualityComparer<T> comparer)
        {
            var map = new Dictionary<T, bool>(comparer);
            foreach (var cur in enumerable)
            {
                if (!map.ContainsKey(cur))
                {
                    map[cur] = true;
                    yield return cur;
                }
            }
        }

        #endregion

        #region Union

        /// <summary>
        /// Union of the two lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerableLeft"></param>
        /// <param name="enumerableRight"></param>
        /// <returns></returns>
        public static IEnumerable<T> Union<T>(this IEnumerable<T> enumerableLeft, IEnumerable<T> enumerableRight)
        {
            return Union(enumerableLeft, enumerableRight, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Union of the two lists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerableLeft"></param>
        /// <param name="enumerableRight"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<T> Union<T>(this IEnumerable<T> enumerableLeft, IEnumerable<T> enumerableRight, IEqualityComparer<T> comparer)
        {
            if (enumerableLeft == null)
            {
                throw new ArgumentNullException("enumerableLeft");
            }

            if (enumerableRight == null)
            {
                throw new ArgumentNullException("enumerableRight");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            var both = enumerableLeft.Concat(enumerableRight);
            return both.Distinct(comparer);
        }

        #endregion

        #region Intersect

        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            return Intersect(left, right, EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> left, IEnumerable<T> right, IEqualityComparer<T> comparer)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            return IntersectHelper(left, right, comparer);
        }

        private static IEnumerable<T> IntersectHelper<T>(IEnumerable<T> left, IEnumerable<T> right, IEqualityComparer<T> comparer)
        {
            var map = new Dictionary<T, bool>(comparer);
            foreach (var cur in left)
            {
                map[cur] = true;
            }

            foreach (var cur in right)
            {
                if (map.ContainsKey(cur) && map[cur])
                {
                    map[cur] = false;
                    yield return cur;
                }
            }
        }

        #endregion

        #region Except

        public static IEnumerable<T> Except<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            return Except(left, right, EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> left, IEnumerable<T> right, IEqualityComparer<T> comparer)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            return ExceptHelper(left, right, comparer);
        }

        private static IEnumerable<T> ExceptHelper<T>(IEnumerable<T> left, IEnumerable<T> right, IEqualityComparer<T> comparer)
        {
            var map = new Dictionary<T, bool>(comparer);
            foreach (var cur in right)
            {
                map[cur] = true;
            }

            foreach (var cur in left)
            {
                if (!map.ContainsKey(cur))
                {
                    yield return cur;
                }
            }
        }

        #endregion

        #region Aggregate

        public static T Aggregate<T>(this IEnumerable<T> source, Func<T, T, T> func)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return AggregateHelper(source, func);
        }

        private static T AggregateHelper<T>(this IEnumerable<T> source, Func<T, T, T> func)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    throw EnumerableExtra.CreateEmpty();
                }

                var result = e.Current;
                while (e.MoveNext())
                {
                    result = func(result, e.Current);
                }

                return result;
            }
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return AggregateHelper(source, seed, func);
        }

        private static TAccumulate AggregateHelper<TSource, TAccumulate>(this IEnumerable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            var result = seed;
            foreach (var cur in source)
            {
                result = func(result, cur);
            }
            return result;
        }

        #endregion

        #region Reverse

        public static IEnumerable<T> Reverse<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            return ReverseHelper(enumerable);
        }

        private static IEnumerable<T> ReverseHelper<T>(this IEnumerable<T> enumerable)
        {
            var list = ImmutableLinkedList<T>.Empty;
            foreach (var cur in enumerable)
            {
                list = list.AddFront(cur);
            }

            return list;
        }

        #endregion

        #endregion

        #region Count

        public static int Count<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            int count = 0;
            using (var e = enumerable.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    ++count;
                }
            }

            return count;
        }

        #endregion

        #region Converters


        public static List<T> ToList<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            return new List<T>(enumerable);
        }

        public static T[] ToArray<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ToList().ToArray();
        }

        #endregion

        #region Cast

        public static IEnumerable<T> Cast<T>(this System.Collections.IEnumerable enumerable)
        {
            var destType = typeof(T);
            foreach (var cur in enumerable)
            {
                yield return (T)cur;
            }
        }

        #endregion

        #region SequenceEqual

        public static bool SequenceEqual<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            return SequenceEqual(left, right, EqualityComparer<T>.Default);
        }

        public static bool SequenceEqual<T>(this IEnumerable<T> left, IEnumerable<T> right, EqualityComparer<T> comparer)
        {
            IEnumerator<T> leftEnum = null;
            IEnumerator<T> rightEnum = null;

            try
            {
                leftEnum = left.GetEnumerator();
                rightEnum = right.GetEnumerator();

                while (leftEnum.MoveNext())
                {
                    if (!rightEnum.MoveNext() || !comparer.Equals(leftEnum.Current, rightEnum.Current))
                    {
                        return false;
                    }
                }

                if (rightEnum.MoveNext())
                {
                    return false;
                }
            }
            finally
            {
                if (leftEnum != null)
                {
                    leftEnum.Dispose();
                }

                if (rightEnum != null)
                {
                    rightEnum.Dispose();
                }
            }

            return true;
        }

        #endregion

        #region Misc

        public static IEnumerable<int> Range(int start, int count)
        {
            long test = (start + (long)count) - 1L;
            if (count < 0 || test > (long)Int32.MaxValue)
            {
                throw EnumerableExtra.CreateOutOfRange("count");
            }

            return RangeHelper(start, count);
        }

        private static IEnumerable<int> RangeHelper(int start, int count)
        {
            int limit = start + count;
            for (int i = start; i < limit; i++)
            {
                yield return i;
            }
        }

        public static T ElementAt<T>(this IEnumerable<T> enumerable, int count)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            using (var e = enumerable.GetEnumerator())
            {
                int cur = 0;
                do
                {
                    if (!e.MoveNext())
                    {
                        throw EnumerableExtra.CreateOutOfRange("count");
                    }
                    cur++;
                }
                while (cur <= count);

                return e.Current;
            }
        }

        public static IEnumerable<T> Repeat<T>(T element, int count)
        {
            if (count < 0)
            {
                throw EnumerableExtra.CreateOutOfRange("count");
            }

            return RepeatHelper(element, count);
        }

        private static IEnumerable<T> RepeatHelper<T>(T element, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return element;
            }
        }

        public static IEnumerable<T> Empty<T>()
        {
            return ImmutableArray<T>.Empty;
        }

        #endregion

    }

#endif

}