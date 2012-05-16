using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras
{
    #region Singleton

    /// <summary>
    /// Used for classes that are single instances per appdomain
    /// </summary>
    public static class Singleton
    {
        private static class Storage<T>
        {
            internal static T s_instance;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2002")]
        public static T GetInstance<T>(Func<T> op)
        {
            if (Storage<T>.s_instance == null)
            {
                lock (typeof(Storage<T>))
                {
                    if (Storage<T>.s_instance == null)
                    {
                        T temp = op();
                        System.Threading.Thread.MemoryBarrier();
                        Storage<T>.s_instance = temp;
                    }
                }
            }
            return Storage<T>.s_instance;
        }

        public static T GetInstance<T>()
            where T : new()
        {
            return GetInstance(() => new T());
        }
    }

    #endregion
}
