using System;
using System.Collections.Generic;
using System.Text;
using BclExtras.Collections;
using System.Diagnostics.CodeAnalysis;
#if NETFX_35
using System.Linq;
#endif

namespace BclExtras
{
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            var temp = Enum.GetValues(typeof(T));
            return temp.Cast<T>();
        }
    }
}
