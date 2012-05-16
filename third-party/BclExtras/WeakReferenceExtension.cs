using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras
{
    public static class WeakReferenceExtension
    {
        public static Option<object> MaybeGetTarget(this WeakReference weakReference)
        {
            if (weakReference == null)
            {
                throw new ArgumentNullException("weakReference");
            }

            object value = weakReference.Target;
            if (value == null)
            {
                return Option.Empty;
            }

            return Option.Create(value);
        }
    }
}
