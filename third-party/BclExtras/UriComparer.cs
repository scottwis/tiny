using System;
using System.Collections.Generic;
using System.Text;
using BclExtras.Threading;

namespace BclExtras
{

    /// <summary>
    /// Class used to compare URI's
    /// </summary>
    [Immutable]
    [Serializable]
    public class UriComparer : IComparer<Uri>
    {
        private static readonly UriComparer s_instance = new UriComparer();

        public static UriComparer Instance
        {
            get { return s_instance; }
        }

        private UriComparer()
        {
        }

        /// <summary>
        /// Compare the two URI's
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int Compare(Uri left, Uri right)
        {
            if (left != null && right == null)
            {
                return 1;
            }
            else if (left == null && right != null)
            {
                return -1;
            }
            else if (left == right)
            {
                return 0;
            }
            else
            {
                return String.CompareOrdinal(left.ToString(), right.ToString());
            }
        }

        #region IComparer<Uri> Members

        int IComparer<Uri>.Compare(Uri x, Uri y)
        {
            return UriComparer.Compare(x, y);
        }

        #endregion
    }
}
