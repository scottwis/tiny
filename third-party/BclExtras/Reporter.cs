using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Diagnostics.CodeAnalysis;
using BclExtras.Threading;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BclExtras
{
    #region LogItemType

    /// <summary>
    /// Type of event that occurred
    /// </summary>
    public enum LogItemType
    {
        /// <summary>
        /// Used to indicate the lowest level possible
        /// </summary>
        None,

        /// <summary>
        /// Used for debug messages
        /// </summary>
        Debug,

        /// <summary>
        /// Tracing information
        /// </summary>
        Trace,

        /// <summary>
        /// Used for times when an exception occurs but it's expected that it
        /// sometimes will.  For example, Querying for persisted data is an 
        /// operation that will sometimes fail because the data could have
        /// changed in such a way on the server as to not allow for the user
        /// to access it.
        /// </summary>
        Warning,

        /// <summary>
        /// Used for unexpected exceptions or events.
        /// </summary>
        Error,
    }

    #endregion

    #region LogItem

    /// <summary>
    /// Information about the error
    /// </summary>
    [Immutable]
    public sealed class LogItem 
    {
        private readonly string m_message;
        private readonly LogItemType m_logType;
        private readonly string m_category;
        private readonly string m_exceptionInfo;

        /// <summary>
        /// Stack trace of the log event (if an exception occurred)
        /// </summary>
        public string ExceptionInformation
        {
            get { return m_exceptionInfo; }
        }

        /// <summary>
        /// Summary of the event
        /// </summary>
        public string Message
        {
            get { return m_message; }
        }

        /// <summary>
        /// Category of the event
        /// </summary>
        public string Category
        {
            get { return m_category; }
        }

        /// <summary>
        /// Whether or not this item is in the default category
        /// </summary>
        public bool IsDefaultCategory
        {
            get { return 0 == String.CompareOrdinal(LogItemCategory.Default, this.Category); }
        }

        /// <summary>
        /// Type of event
        /// </summary>
        public LogItemType LogItemType
        {
            get { return m_logType; }
        }

        public LogItem(LogItemType logType, string category, string summary, Exception exception)
        {
            Debug.Assert(!String.IsNullOrEmpty(summary));
            Debug.Assert(!String.IsNullOrEmpty(category));

            // Set the basic information
            m_logType = logType;
            m_message = !String.IsNullOrEmpty(summary) ? summary : String.Empty;
            m_category = !String.IsNullOrEmpty(category) ? category : String.Empty;

            m_exceptionInfo = CreateExceptionInfo(exception);
        }

        private static string CreateExceptionInfo(Exception e)
        {
            if (null != e)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("Exception List");
                builder.Append(Environment.NewLine);
                Exception current = e;
                int i = 0;
                while (current != null)
                {
                    builder.AppendFormat("{0} {1}: {2}",
                        i++,
                        current.GetType().Name,
                        current.Message);
                    builder.Append(Environment.NewLine);
                    builder.Append(current.StackTrace);
                    current = current.InnerException;
                }

                return builder.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

    }

    #endregion

    #region Logger 

    public sealed class LogItemEventArgs : EventArgs
    {
        private LogItem m_item;
        private Exception m_ex;

        public LogItem LogItem
        {
            get { return m_item; }
        }

        public Exception Exception
        {
            get { return m_ex; }
        }

        internal LogItemEventArgs(LogItem item, Exception ex)
        {
            m_item = item;
            m_ex = ex;
        }
    }

    /// <summary>
    /// Common LogItem Category values
    /// </summary>
    public static class LogItemCategory
    {
        /// <summary>
        /// Used if nothing else is provided
        /// </summary>
        public const string Default = "Default";

        /// <summary>
        /// Used for the Silent category
        /// </summary>
        public const string Silent = "Silent";

        /// <summary>
        /// Used for a reporter error
        /// </summary>
        public const string ReporterError = "ReporterError";

        /// <summary>
        /// Critical Errors
        /// </summary>
        public const string CriticalError = "CriticalError";
    }

    /// <summary>
    /// Responsible for reporting unexpected errors for the developers to debug.  Static class to make
    /// error reporting simple so I won't shun using it
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Raised when a log event is detected.  This is raised on the same thread that the event occurred on
        /// </summary>
        public static event EventHandler<LogItemEventArgs> LogEventCaptured;

        #region Trace

        [Conditional("TRACE")]
        public static void Trace(string message)
        {
            LogCore(LogItemType.Trace, null, message, null);
        }

        [Conditional("TRACE")]
        public static void Trace(string message, Exception ex)
        {
            LogCore(LogItemType.Trace, null, message, ex);
        }

        [Conditional("TRACE")]
        public static void Trace(string category, string message)
        {
            LogCore(LogItemType.Trace, category, message, null);
        }

        [Conditional("TRACE")]
        public static void Trace(string category, string message, Exception ex)
        {
            LogCore(LogItemType.Trace, category, message, ex);
        }

        #endregion

        #region Debug

        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            LogCore(LogItemType.Debug, null, message, null);
        }

        [Conditional("DEBUG")]
        public static void Debug(string message, Exception ex)
        {
            LogCore(LogItemType.Debug, null, message, ex);
        }

        [Conditional("DEBUG")]
        public static void Debug(string category, string message)
        {
            LogCore(LogItemType.Debug, category, message, null);
        }

        [Conditional("DEBUG")]
        public static void Debug(string category, string message, Exception ex)
        {
            LogCore(LogItemType.Debug, category, message, ex);
        }

        #endregion

        #region Warning

        public static void Warning(string message)
        {
            LogCore(LogItemType.Warning, null, message, null);
        }

        public static void Warning(string message, Exception ex)
        {
            LogCore(LogItemType.Warning, null, message, ex);
        }

        public static void Warning(string category, string message)
        {
            LogCore(LogItemType.Warning, category, message, null);
        }

        public static void Warning(string category, string message, Exception ex)
        {
            LogCore(LogItemType.Warning, category, message, ex);
        }

        #endregion

        #region Error

        public static void Error(string message)
        {
            LogCore(LogItemType.Error, null, message, null);
        }

        public static void Error(string message, Exception ex)
        {
            LogCore(LogItemType.Error, null, message, ex);
        }

        public static void Error(string category, string message)
        {
            LogCore(LogItemType.Error, category, message, null);
        }

        public static void Error(string category, string message, Exception ex)
        {
            LogCore(LogItemType.Error, category, message, ex);
        }

        #endregion

        public static void Log(LogItemType type, string category, string message, Exception ex)
        {
            LogCore(type, category, message, ex); 
        }

        private static void LogCore(LogItemType type, string category, string message, Exception ex)
        {
            if (null == category)
            {
                category = LogItemCategory.Default;
            }

            Contract.ThrowIfNullOrEmpty(message);
            Log(new LogItem(type, category, message, ex), ex);
        }

        private static void Log(LogItem info, Exception ex)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(info.Message));
            OnLogEventCaptured(new LogItemEventArgs(info, ex));
        }

        private static void OnLogEventCaptured(LogItemEventArgs info)
        {
            // Cache the value because events are not thread safe.
            EventHandler<LogItemEventArgs> handler = LogEventCaptured;
            if (handler == null)
            {
                return;
            }

            try
            {
                handler(null, info);
            }
            catch (Exception e)
            {
                // If we are in the process of reporting an ReportError and we get
                // an exception, don't rethrow.  It's possible that one of the handlers
                // is in a terrible state.  Rethrowing might lead to an infinite loop.
                if (0 == String.CompareOrdinal(LogItemCategory.ReporterError, info.LogItem.Category))
                {
                    return;
                }

                LogItem info2 = new LogItem(
                    LogItemType.Error,
                    LogItemCategory.ReporterError,
                    "Error handling routine threw an exception during processing.",
                    e);
                Log(info2, e);
            }
        }

    }

    #endregion

}