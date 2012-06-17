// FluentAsserts.cs
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

namespace Tiny
{
    public static class FluentAsserts
    {
        public static T AssumeGT<T>(this T lhs, T rhs) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) <= 0) {
                throw new InternalErrorException(String.Format("Expected a value > {0}", rhs));
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

        public static T AssumeDefined<T>(this T value, string message)
        {
            if (!Enum.IsDefined(typeof (T), value)) {
                throw new InternalErrorException(message);
            }
            return value;
        }

        public static T CheckDefined<T>(this T value, string parameterName)
        {
            if (!Enum.IsDefined(typeof (T), value)) {
                throw new ArgumentOutOfRangeException(parameterName);
            }
            return value;
        }

        public static unsafe void* CheckNotNull(void* pData, string parameterName)
        {
            if (pData == null) {
                throw new ArgumentNullException(parameterName);
            }
            return pData;
        }

        public static T CheckNotNull<T>(this T obj, string parameterName) where T : class
        {
            if (obj == null) {
                throw new ArgumentNullException(parameterName);
            }
            return obj;
        }

        public static T CheckNull<T>(this T obj, string parameterName, string message) where T : class
        {
            if (obj != null)
            {
                throw new ArgumentException(message, parameterName);
            }
            return null;
        }

        public static T CheckGTE<T>(this T lhs, T rhs, string parameterName) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) < 0) {
                throw new ArgumentOutOfRangeException(parameterName, String.Format("Expected a value >= {0}", rhs));
            }
            return lhs;
        }

        public static T CheckLTE<T>(this T lhs, T rhs, string parameterName) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) > 0) {
                throw new ArgumentOutOfRangeException(parameterName, String.Format("Expected a value <= {0}", rhs));
            }
            return lhs;
        }

        public static T CheckLT<T>(this T lhs, T rhs, string parameterName) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) >= 0) {
                throw new ArgumentOutOfRangeException(parameterName, String.Format("Expected a value < {0}", rhs));
            }
            return lhs;
        }

        public static T Check<T>(this T value, bool condition, string message, string parameterName)
        {
            if (!condition) {
                throw new ArgumentException(message, parameterName);
            }
            return value;
        }

        public static T Check<T>(this T value, Func<T, bool> predicate, string message, string parameterName)
        {
            return Check(value, predicate(value), message, parameterName);
        }

        public static T AssumeIs<T>(this object o) where T : class
        {
            var ret = o as T;
            if (ret == null) {
                throw new InternalErrorException(String.Format("Expected an object of type '{0}'", typeof (T)));
            }
            return ret;
        }

        public static bool AssumeTrue(this bool b, string message)
        {
            if (! b) {
                throw new InternalErrorException(message);
            }
            return true;
        }
    }
}
