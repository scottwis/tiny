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
            return AssumeGT(lhs, rhs, () => String.Format("Expected a value > {0}", rhs));
        }

        public static T AssumeGT<T>(this T lhs, T rhs, string message) where T : IComparable<T>
        {
            return AssumeGT(lhs, rhs, () => message);
        }

        public static T AssumeGT<T>(this T lhs, T rhs, Func<string> message) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) <= 0) {
                throw new InternalErrorException(message());
            }
            return lhs;
        }

        public static T AssumeGTE<T>(this T lhs, T rhs) where T : IComparable<T>
        {
            return AssumeGTE(lhs, rhs, () => String.Format("Expected a value >= {0}", rhs));
        }

        public static T AssumeGTE<T>(this T lhs, T rhs, string message) where T : IComparable<T>
        {
            return AssumeGTE(lhs, rhs, () => message);
        }

        public static T AssumeGTE<T>(this T lhs, T rhs, Func<string> message) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) < 0) {
                throw new InternalErrorException(message());
            }
            return lhs;
        }

        public static T AssumeLT<T>(this T lhs, T rhs) where T : IComparable<T>
        {
            return AssumeLT(lhs, rhs, () => String.Format("Expected a value < {0}", rhs));
        }

        public static T AssumeLT<T>(this T lhs, T rhs, String message) where T : IComparable<T>
        {
            return AssumeLT(lhs, rhs, () => message);
        }

        public static T AssumeLT<T>(this T lhs, T rhs, Func<string> message) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) >= 0) {
                throw new InternalErrorException(message());
            }
            return lhs;
        }

        public static T AssumeLTE<T>(this T lhs, T rhs) where T : IComparable<T>
        {
            return AssumeLTE<T>(lhs, rhs, () => String.Format("Expected a value <= {0}.", rhs));
        }

        public static T AssumeLTE<T>(this T lhs, T rhs, string message) where T : IComparable<T>
        {
            return AssumeLTE(lhs, rhs, () => message);
        }

        public static T AssumeLTE<T>(this T lhs, T rhs, Func<string> message) where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) > 0) {
                throw new InternalErrorException(message());
            }
            return lhs;
        }

        public static T AssumeEQ<T>(this T lhs, T rhs) where T : IEquatable<T>
        {
            return AssumeEQ(lhs, rhs, () => String.Format("Expected a value == {0}", rhs));
        }

        public static T AssumeEQ<T>(this T lhs, T rhs, Func<string> message) where T : IEquatable<T>
        {
            if (!lhs.Equals(rhs)) {
                throw new InternalErrorException(message());
            }
            return lhs;
        }

        public static T AssumeEQ<T>(this T lhs, T rhs, string message) where T : IEquatable<T>
        {
            return AssumeEQ(lhs, rhs, () => message);
        }


        public static unsafe void* AssumeNotNull(void* pValue)
        {
            return AssumeNotNull(pValue, () => "Unexpected null pValue.");
        }

        public static unsafe void* AssumeNotNull(void* pValue, string message)
        {
            return AssumeNotNull(pValue, () => message);
        }

        public static unsafe void* AssumeNotNull(void* pValue, Func<string> message)
        {
            if (pValue == null) {
                throw new InternalErrorException(message());
            }
            return pValue;
        }

        public static IntPtr AssumeNotNull(this IntPtr value)
        {
            return AssumeNotNull(value, () => "Unexpected null value.");
        }

        public static IntPtr AssumeNotNull(this IntPtr value, string message)
        {
            return AssumeNotNull(value, () => message);
        }

        public static IntPtr AssumeNotNull(this IntPtr value, Func<string> message)
        {
            if (value == default(IntPtr)) {
                throw new InternalErrorException(message());
            }
            return value;
        }

        public static T AssumeNotNull<T>(this T value) where T : class
        {
            return AssumeNotNull(value, () => "Unexpected null value.");
        }

        public static T AssumeNotNull<T>(this T value, string message) where T : class
        {
            return AssumeNotNull(value, () => message);
        }

        public static T AssumeNotNull<T>(this T value, Func<string> message ) where T : class
        {
            if (value == null) {
                throw new InternalErrorException(message());
            }
            return value;
        }

        public static T AssumeDefined<T>(this T value)
        {
            return AssumeDefined(value, () => "Invalid enum value.");
        }

        public static T AssumeDefined<T>(this T value, string message)
        {
            return AssumeDefined(value, () => message);
        }

        public static T AssumeDefined<T>(this T value, Func<string> message)
        {
            if (!Enum.IsDefined(typeof(T), value)) {
                throw new InternalErrorException(message());
            }
            return value;
        }

        public static T AssumeIs<T>(this object o) where T : class
        {
            return AssumeIs<T>(o, () => String.Format("Expected an object of type '{0}'", typeof (T)));
        }

        public static T AssumeIs<T>(this object o, string message) where T : class
        {
            return AssumeIs<T>(o, () => message);
        }

        public static T AssumeIs<T>(this object o, Func<string> message) where T : class
        {
            var ret = o as T;
            if (ret == null) {
                throw new InternalErrorException(message());
            }
            return ret;
        }

        public static bool Assume(bool condition)
        {
            return Assume(condition, ()=>"Unexpected assertion failure");
        }

        public static bool Assume(this bool b, string message)
        {
            return Assume(b, () => message);
        }

        public static bool Assume(this bool b, Func<string> message)
        {
            if (!b) {
                throw new InternalErrorException(message());
            }
            return true;
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
    }
}
