using System;
using System.Collections.Generic;
using System.Text;
using BclExtras.Threading;

namespace BclExtras
{
    [Immutable]
    [Serializable]
    public sealed class ComparisonUtility<T> : IComparer<T>
    {
        private readonly Comparison<T> m_comp;

        public ComparisonUtility(Comparison<T> comp)
        {
            m_comp = comp;
        }

        #region IComparer<T> Members

        public int Compare(T x, T y)
        {
            return m_comp(x, y);
        }

        #endregion
    }

    public static class ComparisonUtility
    {
        public static ComparisonUtility<T> Create<T>(Func<T, T, int> func)
        {
            return new ComparisonUtility<T>((x, y) => func(x, y));
        }
    }
}
