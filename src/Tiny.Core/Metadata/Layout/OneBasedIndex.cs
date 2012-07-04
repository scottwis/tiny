// OneBasedIndex.cs
//  
// Author:
//     Scott Wisniewski <scott@scottdw2.com>
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

namespace Tiny.Metadata.Layout
{
    //# Defines a wrapper around a 1-based index stored in a meta-data table. Metadata tables used 1 based
    //# "foreign keys" so that "0" can denote the null value. However, we need 0 based indexes for looking up things
    //# in our internal data structures, and for making the "public" API not suck. To avoid a lot of off by one errors
    //# due to conversion between 1-based and 0-based indexes (several of which I've already made) I elicit the help
    //# of the type system, by making the accidental comparison between an 1-based index and a 0-based index a 
    //# compile-time error. See [ZeroBasedIndex] for more info.
    struct OneBasedIndex : IEquatable<OneBasedIndex>, IComparable<OneBasedIndex>, IComparable
    {
        public readonly uint Value;

        public OneBasedIndex(uint value)
        {
            Value = value;
        }

        public static explicit operator ZeroBasedIndex(OneBasedIndex x)
        {
            if (x.Value == 0 || x.Value > int.MaxValue + 1u) {
                throw new InvalidCastException();
            }
            return new ZeroBasedIndex((int) (x.Value - 1));
        }

        public static explicit operator OneBasedIndex(ZeroBasedIndex x)
        {
            if (x.Value < 0) {
                throw new InvalidCastException();
            }
            return new OneBasedIndex((uint) x.Value + 1);
        }

        public static OneBasedIndex operator &(OneBasedIndex lhs, uint rhs)
        {
            return new OneBasedIndex(lhs.Value & rhs);
        }

        public static OneBasedIndex operator | (OneBasedIndex lhs, uint rhs)
        {
            return new OneBasedIndex(lhs.Value | rhs);
        }

        public static OneBasedIndex operator >>(OneBasedIndex lhs, int rhs)
        {
            return new OneBasedIndex(lhs.Value >> rhs);
        }

        public static OneBasedIndex operator <<(OneBasedIndex lhs, int rhs)
        {
            return new OneBasedIndex(lhs.Value << rhs);
        }

        public static bool operator ==(OneBasedIndex lhs, uint rhs)
        {
            return lhs.Value == rhs;
        }

        public static bool operator ==(OneBasedIndex lhs, OneBasedIndex rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator != (OneBasedIndex lhs, uint rhs)
        {
            return lhs.Value != rhs;
        }

        public static bool operator !=(OneBasedIndex lhs, OneBasedIndex rhs)
        {
            return lhs.Value != rhs.Value;
        }

        public int CompareTo(OneBasedIndex other)
        {
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(obj);
        }

        public override bool Equals(object obj)
        {
            return obj is OneBasedIndex && Equals((OneBasedIndex) obj);
        }

        public bool Equals(OneBasedIndex rhs)
        {
            return Value == rhs.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
