using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using BclExtras;

#if NETFX_35
using System.Linq;
#endif

namespace BclExtras.Collections
{
    public static partial class EnumerableExtra
    {
        public static void CopyTo<T>(this IEnumerable<T> enumerable, T[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (var cur in enumerable)
            {
                array[index] = cur;
                ++index;
            }
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T value)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            foreach (var cur in enumerable)
            {
                yield return cur;
            }

            yield return value;
        }

        #region Unfold

        public static IEnumerable<TResult> Unfold<TSource, TResult>(
            TSource state,
            Func<TSource, Option<Tuple<TResult, TSource>>> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("func");
            }

            return UnfoldHelper(state, func);
        }

        private static IEnumerable<TResult> UnfoldHelper<TSource, TResult>(
            TSource state,
            Func<TSource, Option<Tuple<TResult, TSource>>> func)
        {
            do
            {
                var result = func(state);
                if (!result.HasValue)
                {
                    break;
                }

                yield return result.Value.First;
                state = result.Value.Second;
            } while (true);
        }

        #endregion

        #region Zip

        public static IEnumerable<Tuple<TLeft, TRight>> Zip<TLeft, TRight>(this IEnumerable<TLeft> leftEnumerable, IEnumerable<TRight> rightEnumerable)
        {
            if (leftEnumerable == null)
            {
                throw new ArgumentNullException("leftEnumerable");
            }

            if (rightEnumerable == null)
            {
                throw new ArgumentNullException("rightEnumerable");
            }

            return ZipHelper(leftEnumerable, rightEnumerable);
        }

        private static IEnumerable<Tuple<TLeft, TRight>> ZipHelper<TLeft, TRight>(this IEnumerable<TLeft> leftEnumerable, IEnumerable<TRight> rightEnumerable)
        {
            using (var eLeft = leftEnumerable.GetEnumerator())
            {
                using (var eRight = rightEnumerable.GetEnumerator())
                {
                    while (true)
                    {
                        if (!eLeft.MoveNext())
                        {
                            if (eRight.MoveNext())
                            {
                                throw CreateBadLegngth();
                            }
                            break;
                        }
                        else if (!eRight.MoveNext())
                        {
                            throw CreateBadLegngth();
                        }

                        yield return Tuple.Create(eLeft.Current, eRight.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Zip but allows for lists of differing lengths.  The final sequence will have the length of the maximum 
        /// length input.  Default values will be used for missing values
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="leftEnumerable"></param>
        /// <param name="rightEnumerable"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<TLeft, TRight>> ZipExtend<TLeft, TRight>(this IEnumerable<TLeft> leftEnumerable, IEnumerable<TRight> rightEnumerable)
        {
            if (leftEnumerable == null)
            {
                throw new ArgumentNullException("leftEnumerable");
            }

            if (rightEnumerable == null)
            {
                throw new ArgumentNullException("rightEnumerable");
            }

            return ZipExtendHelper(leftEnumerable, rightEnumerable);
        }

        private static IEnumerable<Tuple<TLeft, TRight>> ZipExtendHelper<TLeft, TRight>(this IEnumerable<TLeft> leftEnumerable, IEnumerable<TRight> rightEnumerable)
        {
            using (var eLeft = leftEnumerable.GetEnumerator())
            {
                using (var eRight = rightEnumerable.GetEnumerator())
                {
                    while (true)
                    {
                        if (!eLeft.MoveNext())
                        {
                            while (eRight.MoveNext())
                            {
                                yield return Tuple.Create(default(TLeft), eRight.Current);
                            }
                            break;
                        }
                        else if (!eRight.MoveNext())
                        {
                            do
                            {
                                yield return Tuple.Create(eLeft.Current, default(TRight));
                            } while (eLeft.MoveNext());
                            break;
                        }

                        yield return Tuple.Create(eLeft.Current, eRight.Current);
                    }
                }
            }
        }

        /// <summary>
        /// Zip but allows for lists of differing lengths.  The final sequence will have the length of the minimum 
        /// length input.  Extra values in the longer list are ignored
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="leftEnumerable"></param>
        /// <param name="rightEnumerable"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<TLeft, TRight>> ZipShorten<TLeft, TRight>(this IEnumerable<TLeft> leftEnumerable, IEnumerable<TRight> rightEnumerable)
        {
            if (leftEnumerable == null)
            {
                throw new ArgumentNullException("leftEnumerable");
            }

            if (rightEnumerable == null)
            {
                throw new ArgumentNullException("rightEnumerable");
            }

            return ZipShortenHelper(leftEnumerable, rightEnumerable);
        }

        private static IEnumerable<Tuple<TLeft, TRight>> ZipShortenHelper<TLeft, TRight>(this IEnumerable<TLeft> leftEnumerable, IEnumerable<TRight> rightEnumerable)
        {
            using (var eLeft = leftEnumerable.GetEnumerator())
            {
                using (var eRight = rightEnumerable.GetEnumerator())
                {
                    while (true)
                    {
                        if (!eLeft.MoveNext() || !eRight.MoveNext())
                        {
                            break;
                        }
                        yield return Tuple.Create(eLeft.Current, eRight.Current);
                    }
                }
            }
        }

        #endregion

        #region Errors

        internal static InvalidOperationException CreateBadLegngth()
        {
            return new InvalidOperationException("Lists have invalid length");
        }

        internal static InvalidOperationException CreateNoMatch()
        {
            return new InvalidOperationException("No match found");
        }

        internal static InvalidOperationException CreateEmpty()
        {
            return new InvalidOperationException("Enumerable is empty");
        }

        internal static ArgumentOutOfRangeException CreateOutOfRange(string name)
        {
            return new ArgumentOutOfRangeException(name, "Argument is out of range");
        }

        #endregion

        #region FromSingle

        /// <summary>
        /// Return a single element list of type T.  The only value will be the default value
        /// of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> FromSingle<T>()
        {
            return FromSingle(default(T));
        }

        /// <summary>
        /// Return a single element list with the value passed in
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> FromSingle<T>(T value)
        {
            yield return value;
        }
        #endregion

        #region Union Single

        /// <summary>
        /// Union the list with the single value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> Union<T>(this IEnumerable<T> enumerable, T value)
        {
            return Enumerable.Union(enumerable, FromSingle(value));
        }

        /// <summary>
        /// Union the list with the single value given the specified comparison
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="value"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<T> Union<T>(this IEnumerable<T> enumerable, T value, IEqualityComparer<T> comparer)
        {
            return Enumerable.Union(enumerable, FromSingle(value), comparer);
        }

        #endregion

        #region Intersect Single

        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> enumerable, T value)
        {
            return Enumerable.Intersect<T>(enumerable, FromSingle(value), EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> enumerable, T value, IEqualityComparer<T> comparer)
        {
            return Enumerable.Intersect<T>(enumerable, FromSingle(value), comparer);
        }

        #endregion

        #region Except Single

        public static IEnumerable<T> Except<T>(this IEnumerable<T> left, T value)
        {
            return Enumerable.Except(left, FromSingle(value), EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> left, T value, IEqualityComparer<T> comparer)
        {
            return Enumerable.Except(left, FromSingle(value), comparer);
        }


        #endregion

        #region ForEach

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> callback)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            ForEachHelper(enumerable, (x, i) => callback(x));
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T,int> callback)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            ForEachHelper(enumerable, callback);
        }

        private static void ForEachHelper<T>(this IEnumerable<T> enumerable, Action<T,int> callback)
        {
            int count = 0;
            foreach (var cur in enumerable)
            {
                callback(cur, count);
                count++;
            }
        }

        #endregion

        #region SafeCast

        public static IEnumerable<T> SafeCast<T>(this System.Collections.IEnumerable enumerable)
        {
            var destType = typeof(T);
            foreach (var cur in enumerable)
            {
                yield return TypeUtil.Cast<T>(cur, destType);
            }
        }

        #endregion
    }
}