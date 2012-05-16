using System;
using System.Collections.Generic;
using System.Text;
using BclExtras.Threading;

namespace BclExtras
{
    /// <summary>
    /// Occassionally you need an IComparable(Of T) when you only have an IComparer(Of T).  This 
    /// class can be composed to create an IComparable(Of T) with those two values
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Immutable]
    [Serializable]
    public sealed class ComparerNode<T> : IComparable<ComparerNode<T>>
    {
        private readonly IComparer<T> m_comparer;
        private readonly T m_value;

        public IComparer<T> Comparer
        {
            get { return m_comparer; }
        }

        public T Value
        {
            get { return m_value; }
        }

        public ComparerNode(IComparer<T> comparer, T value)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

            m_comparer = comparer;
            m_value = value;
        }

        public override int GetHashCode()
        {
            if (m_value == null)
            {
                return 0;
            }

            return m_value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as ComparerNode<T>;
            if (other == null)
            {
                return false;
            }

            return 0 == m_comparer.Compare(m_value, other.m_value);
        }

        #region IComparable<ComparerNode<T>> Members

        public int CompareTo(ComparerNode<T> other)
        {
            if (other == null)
            {
                return 1;
            }

            return m_comparer.Compare(m_value, other.m_value);
        }

        #endregion

        public static bool operator <(ComparerNode<T> left, ComparerNode<T> right)
        {
            return Comparer<ComparerNode<T>>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(ComparerNode<T> left, ComparerNode<T> right)
        {
            return Comparer<ComparerNode<T>>.Default.Compare(left, right) > 0;
        }

        public static bool operator ==(ComparerNode<T> left, ComparerNode<T> right)
        {
            return EqualityComparer<ComparerNode<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(ComparerNode<T> left, ComparerNode<T> right)
        {
            return !EqualityComparer<ComparerNode<T>>.Default.Equals(left, right);
        }
    }

    public static class ComparerNode
    {
        public static ComparerNode<T> Create<T>(IComparer<T> comparer, T value)
        {
            return new ComparerNode<T>(comparer, value);
        }

        public static ComparerNode<T> Create<T>(T value)
            where T: IComparable<T>
        {
            return new ComparerNode<T>(Comparer<T>.Default, value);
        }
    }
}
