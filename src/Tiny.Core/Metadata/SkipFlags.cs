using System;
using System.Collections.Generic;

namespace Tiny.Metadata
{
    [Flags]
    enum SkipFlags : byte
    {
        TypeCheck = 0x1,
        RangeCheck = 0x2,
        NullCheck = 0x04,

        VALID_FLAGS = 0x7
    }

    static class SkipFlagsExtensions
    {
        public static IEnumerable<String> ToSequence(this SkipFlags flags)
        {
            if ((flags & SkipFlags.TypeCheck) != 0) {
                yield return "typecheck";
            }
            if ((flags & SkipFlags.RangeCheck) != 0) {
                yield return "rangecheck";
            }
            if ((flags & SkipFlags.NullCheck) != 0) {
                yield return "nullcheck";
            }
        }
    }
}
