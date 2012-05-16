using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace BclExtras.Threading
{
    public interface IFuture
    {
        /// <summary>
        /// Whether or not the future has completed
        /// </summary>
        bool HasCompleted
        {
            get;
        }

        /// <summary>
        /// Wait for the future to complete
        /// </summary>
        void WaitEmpty();
    }

    public interface IFuture<T> : IFuture
    {
        /// <summary>
        /// Wait for the future to complete and return the value
        /// </summary>
        /// <returns></returns>
        T Wait();
    }
}
