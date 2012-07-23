using System;

namespace Tiny.Parsing
{
    static class Parsers
    {
        public static IParser<TInput, T1, T3> Then<TInput, T1, T2, T3>(this IParser<TInput, T1,T2> first, IParser<TInput, T2, T3> second)
        {
            throw new NotImplementedException();
        }
    }
}
