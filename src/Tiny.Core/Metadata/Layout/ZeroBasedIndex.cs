// ZeroBasedIndex.cs
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
    struct ZeroBasedIndex : IEquatable<ZeroBasedIndex>, IComparable<ZeroBasedIndex>, IComparable
    {
        public readonly int Value;

        public ZeroBasedIndex(int value)
        {
            Value = value;
        }

        public static bool operator == (ZeroBasedIndex lhs, ZeroBasedIndex rhs)
        {
            return lhs.Value == rhs.Value;
        }

        public static bool operator == (ZeroBasedIndex lhs, int rhs)
        {
            return lhs.Value == rhs;
        }

        public static bool operator != (ZeroBasedIndex lhs, ZeroBasedIndex rhs)
        {
            return lhs.Value != rhs.Value;
        }

        public static bool operator != (ZeroBasedIndex lhs, int rhs)
        {
            return lhs.Value != rhs;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return Value.CompareTo(obj);
        }

        public bool Equals(ZeroBasedIndex other)
        {
            return Value.Equals(other.Value);
        }

        public int CompareTo(ZeroBasedIndex other)
        {
            return Value.CompareTo(other.Value);
        }

        public override bool Equals(object obj)
        {
            return Value.Equals(obj);
        }

        public static ZeroBasedIndex operator - (ZeroBasedIndex lhs, ZeroBasedIndex rhs)
        {
            return new ZeroBasedIndex(lhs.Value - rhs.Value);
        }

        public static ZeroBasedIndex operator -(ZeroBasedIndex lhs, int rhs)
        {
            return new ZeroBasedIndex(lhs.Value - rhs);
        }

        public static bool operator < (ZeroBasedIndex lhs, ZeroBasedIndex rhs)
        {
            return lhs.Value < rhs.Value;
        }

        public static bool operator <(ZeroBasedIndex lhs, int rhs)
        {
            return lhs.Value < rhs;
        }

        public static bool operator >(ZeroBasedIndex lhs, ZeroBasedIndex rhs)
        {
            return lhs.Value > rhs.Value;
        }

        public static bool operator >(ZeroBasedIndex lhs, int rhs)
        {
            return lhs.Value > rhs;
        }

        public static bool operator <=(ZeroBasedIndex lhs, ZeroBasedIndex rhs)
        {
            return lhs.Value <= rhs.Value;
        }

        public static bool operator <=(ZeroBasedIndex lhs, int rhs)
        {
            return lhs.Value <= rhs;
        }

        public static bool operator >=(ZeroBasedIndex lhs, ZeroBasedIndex rhs)
        {
            return lhs.Value >= rhs.Value;
        }

        public static bool operator >=(ZeroBasedIndex lhs, int rhs)
        {
            return lhs.Value >= rhs;
        }

        public static ZeroBasedIndex operator +(ZeroBasedIndex lhs, int rhs)
        {
            return new ZeroBasedIndex(lhs.Value + rhs);
        }

        public static ZeroBasedIndex operator +(ZeroBasedIndex lhs, ZeroBasedIndex rhs)
        {
            return new ZeroBasedIndex(lhs.Value + rhs.Value);
        }

        public static ZeroBasedIndex operator / (ZeroBasedIndex lhs, ZeroBasedIndex rhs)
        {
            return new ZeroBasedIndex(lhs.Value / rhs.Value);
        }

        public static ZeroBasedIndex operator /(ZeroBasedIndex lhs, int rhs)
        {
            return new ZeroBasedIndex(lhs.Value / rhs);
        }
    }
}
