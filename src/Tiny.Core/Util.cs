// 
// Util.cs
//  
// Author:
//       Scott Wisniewski <scott@scottdw2.com>
// 
// Copyright (c) 2012 Scott Wisniewski
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tiny
{
    public static class Util
    {
        public static IDictionary<K,V> AsReadOnly<K, V>(this IDictionary<K,V> d)
        {
            d.CheckNotNull("d");
            return new ReadOnlyDictionary<K, V>(d.CheckNotNull("d"));
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
        {
            var ret = new HashSet<T>();
            items.ForEach(x=>ret.Add(x));
            return ret;
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            items.CheckNotNull("items");
            action.CheckNotNull("action");

            foreach (var item in items) {
                action(item);
            }
        }

        public static uint Pad(this uint length, uint pad)
        {
            return checked(((pad - (length%pad))%pad) + length);
        }

        public static int BitCount(this ulong value)
        {
            //# This code is a little complex, but it is really fast. Ideally we would just use popcnt, but it's not
            //# available to us from C#.
            //#
            //# See the following for references:
            //# 1. http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSet64
            //# 2. http://www.necessaryandsufficient.net/2009/04/optimising-bit-counting-using-iterative-data-driven-development/
            //# 3. http://en.wikipedia.org/wiki/Hamming_weight
            //# 4. http://www-cs-faculty.stanford.edu/~knuth/fasc1a.ps.gz
            const ulong m1 = 0x5555555555555555UL;
            const ulong m2 = 0x3333333333333333UL;
            const ulong m4 = 0x0F0F0F0F0F0F0F0FUL;

            value -= ((value >> 1) & m1);
            value = ((value >> 2) & m2) + (value & m2);
            value = (value + (value >> 4)) & m4;
            return (int)((value * 0x0101010101010101UL) >> 56);
        }
    }
}

