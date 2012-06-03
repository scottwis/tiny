using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using ThreadTimer = System.Threading.Timer;

namespace BclExtras.Threading
{
    #region Immutable

    /// <summary>
    /// This class does nothing except declaratively say that I want a class
    /// to be immutable.  Test case code verifies it's actually immutable
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class ImmutableAttribute : Attribute
    {
        public ImmutableAttribute()
        {
        }
    }

    #endregion

    #region ThreadSafe

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ThreadSafeAttribute : Attribute
    {
        public ThreadSafeAttribute()
        {
        }
    }
    #endregion

    #region FutureLogCategory

    /// <summary>
    /// Common categories for logging operations
    /// </summary>
    public static class FutureLogCategory
    {
        public const string BeginInvoke = "BeginInvoke";

        public const string Invoke = "Invoke";

        /// <summary>
        /// Reserved for errors that can occur but are often expected to do so
        /// </summary>
        public const string Expected = "Expected";
    }

    #endregion
}