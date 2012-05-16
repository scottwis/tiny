using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras.Threading
{
    #region InvocationException

    [global::System.Serializable]
    public class InvocationException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvocationException() { }
        public InvocationException(string message) : base(message) { }
        public InvocationException(string message, Exception inner) : base(message, inner) { }
        protected InvocationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    #endregion

    #region FutureException

    [global::System.Serializable]
    public class FutureException : InvocationException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public FutureException() { }
        public FutureException(string message) : base(message) { }
        public FutureException(string message, Exception inner) : base(message, inner) { }
        protected FutureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    #endregion

    #region FutureCanceledException

    [global::System.Serializable]
    public class FutureCanceledException : FutureException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //  

        public FutureCanceledException() { }
        public FutureCanceledException(string message) : base(message) { }
        public FutureCanceledException(string message, Exception inner) : base(message, inner) { }
        protected FutureCanceledException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    #endregion

    #region FutureAbortedException

    [global::System.Serializable]
    public class FutureAbortedException : FutureException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public FutureAbortedException() { }
        public FutureAbortedException(string message) : base(message) { }
        public FutureAbortedException(string message, Exception inner) : base(message, inner) { }
        protected FutureAbortedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    #endregion
}
