using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras
{

    /// <summary>
    /// Raised when a Contract violation occurs
    /// </summary>
    [global::System.Serializable]
    public class ContractException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ContractException() : this("Contract Violation") { }
        public ContractException(string message) : base(message) { }
        public ContractException(string message, Exception inner) : base(message, inner) { }
        protected ContractException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Used for internal contract checking
    /// </summary>
    public static class Contract
    {
        /// <summary>
        /// This is a hack but a useful one.  I like to have a Debug.Fail whenever there is a 
        /// contract violation so I immediately see the error when running Cugger.  However
        /// during UnitTesting it prevents me from running them since I have to hang around and
        /// dismiss the assert dialog.  So this is a compromise. 
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211")]
        public static bool InUnitTest;

        /// <summary>
        /// Report a violation
        /// </summary>
        /// <param name="message"></param>
        public static void Violation(string message)
        {
            ContractViolation(message);
        }

        /// <summary>
        /// Report a violation
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Violation(string format, params object[] args)
        {
            ContractViolation(string.Format(format, args));
        }

        /// <summary>
        /// Ensure value is not null
        /// </summary>
        /// <param name="value"></param>
        public static void ThrowIfNotNull(object value)
        {
            ThrowIfNotNull(value, "Value should be null");
        }

        /// <summary>
        /// Ensure value is not null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public static void ThrowIfNotNull(object value, string message)
        {
            if (value != null)
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Ensure value is null
        /// </summary>
        /// <param name="value"></param>
        public static void ThrowIfNull(object value)
        {
            ThrowIfNull(value, "Value should not be null");
        }

        /// <summary>
        /// Ensure value is null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public static void ThrowIfNull(object value, string message)
        {
            if (null == value)
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Ensure value is not false
        /// </summary>
        /// <param name="value"></param>
        public static void ThrowIfFalse(bool value)
        {
            ThrowIfFalse(value, "Unexpected false");
        }

        /// <summary>
        /// Ensure value is not false
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public static void ThrowIfFalse(bool value, string message)
        {
            if (!value)
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Ensure value is a valid non-empty string
        /// </summary>
        /// <param name="value"></param>
        public static void ThrowIfNullOrEmpty(string value)
        {
            ThrowIfNullOrEmpty(value, "String is null or empty");
        }

        /// <summary>
        /// Ensure value is a valid non-empty string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public static void ThrowIfNullOrEmpty(string value, string message)
        {
            if (String.IsNullOrEmpty(value))
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Ensure the value is true
        /// </summary>
        /// <param name="value"></param>
        public static void ThrowIfTrue(bool value)
        {
            ThrowIfTrue(value, "Unexpected true");
        }

        /// <summary>
        /// Ensure the value is true
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public static void ThrowIfTrue(bool value, string message)
        {
            if (value)
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Ensure the value is not negative 
        /// </summary>
        /// <param name="value"></param>
        public static void ThrowIfNegative(int value)
        {
            ThrowIfNegative(value, "Value is negative");
        }

        /// <summary>
        /// Ensure the value is not negative 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        public static void ThrowIfNegative(int value, string message)
        {
            if (value < 0)
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Ensure the values are not equal 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void ThrowIfEqual<T>(T left, T right)
        {
            ThrowIfEqual<T>(left, right, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Ensure the values are not equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="comparer"></param>
        public static void ThrowIfEqual<T>(T left, T right, IEqualityComparer<T> comparer)
        {
            ThrowIfEqual<T>(left, right, comparer, "Values should not be equal");
        }

        /// <summary>
        /// Ensure the values are not equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="comparer"></param>
        /// <param name="message"></param>
        public static void ThrowIfEqual<T>(T left, T right, IEqualityComparer<T> comparer, string message)
        {
            if (comparer.Equals(left, right))
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Ensure the values are equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void ThrowIfNotEqual<T>(T left, T right)
        {
            ThrowIfNotEqual<T>(left, right, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Ensure the values are equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="comparer"></param>
        public static void ThrowIfNotEqual<T>(T left, T right, IEqualityComparer<T> comparer)
        {
            ThrowIfNotEqual<T>(left, right, comparer, "Values should be equal");
        }

        /// <summary>
        /// Ensure the values are equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="comparer"></param>
        /// <param name="message"></param>
        public static void ThrowIfNotEqual<T>(T left, T right, IEqualityComparer<T> comparer, string message)
        {
            if (!comparer.Equals(left, right))
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Ensure the value is of the specified type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004")]
        public static T ThrowIfNotType<T>(object value)
        {
            return ThrowIfNotType<T>(value, "Value is not the correct type");
        }

        /// <summary>
        /// Ensure the value is of the specified type 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004")]
        public static T ThrowIfNotType<T>(object value, string message)
        {
            T newVal = default(T);
            try
            {
                newVal = (T)value;
            }
            catch (InvalidCastException)
            {
                ContractViolation(message);
            }

            return newVal;
        }

        /// <summary>
        /// Ensure we are on the specified thread
        /// </summary>
        /// <param name="threadId"></param>
        public static void EnsureOnThread(int threadId)
        {
            EnsureOnThread(threadId, "Called on wrong thread");
        }

        /// <summary>
        /// Ensure we are on the specified thread
        /// </summary>
        /// <param name="threadId"></param>
        /// <param name="message"></param>
        public static void EnsureOnThread(int threadId, string message)
        {
            if (Thread.CurrentThread.ManagedThreadId != threadId)
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Ensure we are not on the specified thread
        /// </summary>
        /// <param name="threadId"></param>
        public static void EnsureNotOnThread(int threadId)
        {
            EnsureNotOnThread(threadId, "Called on wrong thread");
        }

        /// <summary>
        /// Ensure we are not on the specified thread
        /// </summary>
        /// <param name="threadId"></param>
        /// <param name="message"></param>
        public static void EnsureNotOnThread(int threadId, string message)
        {
            if (Thread.CurrentThread.ManagedThreadId == threadId)
            {
                ContractViolation(message);
            }
        }

        /// <summary>
        /// Check for a bad thread call
        /// </summary>
        /// <param name="invoke"></param>
        [SuppressMessage("Microsoft.Naming", "CA1702")]
        public static void CheckForBadThreadCall(ISynchronizeInvoke invoke)
        {
            if (invoke == null)
            {
                throw new ArgumentNullException("invoke");
            }

            CheckForBadThreadCall(invoke, "Illegal cross thread call");
        }

        /// <summary>
        /// Check for a bad thread call
        /// </summary>
        /// <param name="invoke"></param>
        /// <param name="message"></param>
        [SuppressMessage("Microsoft.Naming", "CA1702")]
        public static void CheckForBadThreadCall(ISynchronizeInvoke invoke, string message)
        {
            if (invoke == null)
            {
                throw new ArgumentNullException("invoke");
            }

            if (invoke.InvokeRequired)
            {
                Violation(message);
            }
        }

        /// <summary>
        /// Throw because we have an invalid enumeration value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public static void InvalidEnumValue<T>(T value)
            where T : struct
        {
            Contract.ThrowIfFalse(typeof(T).IsEnum, "Expected an enum type");

            Violation("Invalid Enum value of Type {0} : {1}", typeof(T).Name, value);
        }

        private static void ContractViolation(string message)
        {
            if (!InUnitTest)
            {
                Debug.Fail("Contract Violation: " + message);
            }

            // Log the failure
            StackTrace trace = new StackTrace();
            string logMessage = message;
            logMessage += Environment.NewLine;
            logMessage += trace.ToString();
            Logger.Error("ContractViolation",logMessage);

            throw new ContractException(message);
        }
    }


}
