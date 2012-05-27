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

namespace Tiny.Decompiler
{
    static class Util
    {
        public static uint AssumeGTZ(this uint x)
        {
            return x.AssumeGT(0U);
        }

        public static T AssumeGT<T>(this T lhs, T rhs) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) <= 0) {
                throw new InternalErrorException(string.Format("Expected a value > {0}", rhs));
            }
            return lhs;
        }

        public static T AssumeGTE<T>(this T lhs, T rhs) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) < 0) {
                throw new InternalErrorException(String.Format("Expected a value >= {0}", rhs));
            }
            return lhs;
        }

        public static T AssumeLT<T>(this T lhs, T rhs) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) >= 0) {
                throw new InternalErrorException(String.Format("Expected a value < {0}", rhs));
            }
            return lhs;
        }

        public static T AssumeLTE<T>(this T lhs, T rhs) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) > 0) {
                throw new InternalErrorException(String.Format("Expected a value <= {0}.", rhs));
            }
            return lhs;
        }

        public static void Assume(bool condition)
        {
            if (! condition) {
                throw new InternalErrorException("Unexpected assertion failure");
            }
        }

        public static unsafe void* AssumeNotNull(void* pValue)
        {
            if (pValue == null) {
                throw new InternalErrorException("Unexpected null value.");
            }
            return pValue;
        }

        public static IntPtr AssumeNotNull(this IntPtr value)
        {
            if (value == null) {
                throw new InternalErrorException("Unexpected null value.");
            }
            return value;
        }

        public static T AssumeNotNull<T>(this T value) where T : class
        {
            if (value == null) {
                throw new InternalErrorException("Unexpected null value.");
            }
            return value;
        }

        public static T CheckDefined<T>(this T value, string message)
        {
            if (!Enum.IsDefined(typeof(T), value)) {
                throw new InvalidOperationException(message);
            }
            return value;
        }

        public static T CheckNull<T>(this T obj, string parameterName) where T : class
        {
            if (obj == null) {
                throw new ArgumentNullException(parameterName);
            }
            return obj;
        }

        public static IDictionary<K,V> AsReadOnly<K, V>(this IDictionary<K,V> d)
        {
            return new ReadOnlyDictionary<K, V>(d.CheckNull("d"));
        }

        public static uint Pad(this uint length, uint pad)
        {
            return checked(((pad - (length%pad))%pad) + length);
        }
    }
}

