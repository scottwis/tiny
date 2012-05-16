using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras
{
    public static class TypeUtil
    {
        internal static T Cast<T>(object source, Type destinationType)
        {
            if (null == source && destinationType.IsPrimitive)
            {
                throw new InvalidCastException();
            }

            if (null == source || source is string || !(source is IConvertible))
            {
                return (T)source;
            }

            return (T)Convert.ChangeType(source, destinationType);
        }
    }
}
