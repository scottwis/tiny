using System;
using BclExtras.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace BclExtras
{
    [Immutable]
    [Serializable]
    [SuppressMessage("Microsoft.Naming", "CA1716")]
    [DebuggerDisplay("HasValue: {m_hasValue}, Value: {m_value}")]
    public struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T m_value;
        private readonly bool m_hasValue;

        public static Option<T> Empty
        {
            get { return new Option<T>(); }
        }

        public bool HasValue
        {
            get { return m_hasValue; }
        }

        [DebuggerDisplay("m_value")]
        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("Option does not have a value");
                }

                return m_value;
            }
        }

        public T ValueOrDefault
        {
            get { return m_hasValue ? m_value : default(T); }
        }

        public Option(T value)
        {
            m_hasValue = true;
            m_value = value;
        }

        #region Operators

        [SuppressMessage("Microsoft.Usage", "CA1801")]
        [SuppressMessage("Microsoft.Usage", "CA2225")]
        public static implicit operator Option<T>(Option option)
        {
            return Option<T>.Empty;
        }

        [SuppressMessage("Microsoft.Usage", "CA1801")]
        public static implicit operator Option<T>(T value)
        {
            return new Option<T>(value);
        }

        public static bool operator ==(Option<T> left, Option<T> right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Option<T> left, Option<T> right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region IEquatable<T> Members

        public bool Equals(Option<T> other)
        {
            if (other.HasValue != this.HasValue)
            {
                return false;
            }

            // Both don't have a value
            if (!other.HasValue)
            {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(m_value, other.Value);
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is Option<T>)
            {
                return Equals((Option<T>)obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (!HasValue)
            {
                return 0;
            }

            return EqualityComparer<T>.Default.GetHashCode(m_value);
        }

        #endregion
    }

    [Immutable]
    [Serializable]
    [SuppressMessage("Microsoft.Naming", "CA1716")]
    public sealed class Option
    {
        private static Option s_empty = new Option();

        private Option()
        {
        }

        public static Option<T> Create<T>(T value)
        {
            return new Option<T>(value);
        }

        public static Option Empty
        {
            get { return s_empty; }
        }
    }
}
