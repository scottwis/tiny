using System;
using System.Collections.Generic;
using BclExtras.Collections;
using BclExtras.Threading;

namespace BclExtras
{
    public static class Match
    {
        public static MatchValue<T> Value<T>(T value)
        {
            return new MatchValue<T>(value);
        }

        public static MatchValue<T> Value<T>(T value, IEqualityComparer<T> comp)
        {
            return new MatchValue<T>(value, comp);
        }
    }

    /// <summary>
    /// Basic pattern matching implementation.  
    /// </summary>
    [Immutable]
    public sealed class MatchValue<T>
    {
        private readonly T m_value;
        private readonly IEqualityComparer<T> m_comp;

        public T Value
        {
            get{ return m_value; }
        }

        public IEqualityComparer<T> EqualityComparer
        {
            get { return m_comp; }
        }

        public MatchValue(T value) : this(value, EqualityComparer<T>.Default)
        {
        }
    
        public MatchValue(T value, IEqualityComparer<T> comp)
        {
            m_value = value;
            m_comp = comp;
        }

        public WithClause<T, TResult> With<TResult>(T valueComp, TResult result)
        {
            return new WithClause<T, TResult>(
                this,
                x => m_comp.Equals(x, valueComp),
                () => result);
        }
    }

    [Immutable]
    public sealed class WithClause<TValue,TResult>
    {
        private readonly MatchValue<TValue> m_value;
        private readonly bool m_success;
        private readonly Func<TResult> m_valueFunc;

        public TResult End
        {
            get
            {
                if (!m_success)
                {
                    throw new InvalidMatchException();
                }

                return m_valueFunc();
            }
        }

        internal WithClause(MatchValue<TValue> value, Func<TValue,bool> predicate, Func<TResult> valueFunc)
        {
            m_value = value;
            m_success = predicate(value.Value);
            m_valueFunc = valueFunc;
        }

        public TResult Default(TResult result)
        {
            if (m_success)
            {
                return this.End;
            }

            return result;
        }

        public WithClause<TValue, TResult> With(TValue compValue, TResult result)
        {
            if (m_success)
            {
                return this;
            }

            return new WithClause<TValue, TResult>(
                m_value,
                x => m_value.EqualityComparer.Equals(x, compValue),
                () => result);
        }
    }

    [global::System.Serializable]
    public class InvalidMatchException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidMatchException() { }
        public InvalidMatchException(string message) : base(message) { }
        public InvalidMatchException(string message, Exception inner) : base(message, inner) { }
        protected InvalidMatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
