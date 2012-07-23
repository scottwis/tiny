using System;

namespace Tiny.Metadata
{
    [Flags]
    enum SkipFlags : byte
    {
        TypeCheck = 0x1,
        RangeCheck = 0x2,
        NullCheck = 0x04
    }
}
