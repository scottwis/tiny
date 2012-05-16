using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras
{
    /// <summary>
    /// C# does not support lambda return type inference.  It makes it impossible to use such types as anonymous
    /// types in explicit lambda expressions.  This code allows for basic lambdas to be created.
    /// 
    /// At first this code may look redundant but it allwos for the following code pattern
    ///   var f = Lambda.Create(() => new { Name = "foo" });
    ///   
    /// </summary>
    public static class Lambda
    {
        public static Func<T> Create<T>(Func<T> del)
        {
            return del;
        }

        public static Func<TReturn, TArg1> Create<TReturn, TArg1>(Func<TReturn, TArg1> del)
        {
            return del;
        }

        public static Func<TArg1, TArg2, TReturn> Create<TReturn, TArg1, TArg2>(Func<TArg1, TArg2, TReturn> del)
        {
            return del;
        }

        public static Func<TArg1, TArg2, TArg3, TReturn> Create<TReturn, TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, TReturn> del)
        {
            return del;
        }

        public static Action Create(Action del)
        {
            return del;
        }

        public static Action<T> Create<T>(Action<T> del)
        {
            return del;
        }

        public static Action<TArg1, TArg2> Create<TArg1,TArg2>(Action<TArg1,TArg2> del)
        {
            return del;
        }

        public static Action<TArg1, TArg2,TArg3> Create<TArg1,TArg2,TArg3>(Action<TArg1,TArg2,TArg3> del)
        {
            return del;
        }
    }
}
