using System;
using System.Collections.Generic;
using BclExtras.Threading;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras
{
    /// <summary>
    /// Several functional extension methods which make it easier to write functional 
    /// style code
    /// </summary>
    public static class Functional
    {
        /// <summary>
        /// Memoize a particular function.  Function can only be called from this thread
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA1704")]
        public static Func<TSource, TResult> Memoize<TSource, TResult>(this Func<TSource, TResult> func)
        {
            var affinity = new ThreadAffinity();
            var map = new Dictionary<TSource, TResult>();
            return x =>
                {
                    affinity.Check();
                    TResult result;
                    if (!map.TryGetValue(x, out result))
                    {
                        result = func(x);
                        map[x] = result;
                    }

                    return result;
                };
        }

        /// <summary>
        /// Return a function which will negate the result of the original function
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<TArg1, bool> Negate<TArg1>(this Func<TArg1,bool> func)
        {
            return (arg1) => !func(arg1);
        }

        /// <summary>
        /// Return a function which will negate the result of the original function
        /// </summary>
        /// <typeparam name="TArg1"></typeparam>
        /// <typeparam name="TArg2"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Func<TArg1, TArg2, bool> Negate<TArg1, TArg2>(this Func<TArg1, TArg2,bool> func)
        {
            return (arg1, arg2) => !func(arg1, arg2);
        }

        public static Func<TArg1, TArg2, TArg3, bool> Negate<TArg1, TArg2, TArg3>(this Func<TArg1, TArg2, TArg3,bool> func)
        {
            return (arg1, arg2, arg3) => !func(arg1, arg2, arg3);
        }

    }
}
