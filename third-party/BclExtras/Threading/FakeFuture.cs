using System;
using System.Collections.Generic;
using System.Text;

namespace BclExtras.Threading
{
    /// <summary>
    /// Used when you need to display a Future but the value doesn't need to be computed asynchronously
    /// </summary>
    public class FakeFuture : IFuture 
    {
        #region IFuture Members

        public bool HasCompleted
        {
            get { return true; } 
        }

        public void WaitEmpty()
        {
            // Do Nothing
        }

        #endregion

        public static FakeFuture CreateEmpty()
        {
            return new FakeFuture();
        }

        public static FakeFuture<T> Create<T>(T value)
        {
            return new FakeFuture<T>(value);
        }
    }

    /// <summary>
    /// Used when you need a Future but you don't need to calculate the value asynchronously
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FakeFuture<T> : IFuture<T>
    {
        private T m_value;

        public FakeFuture(T value)
        {
            m_value = value;
        }

        #region IFuture<T> Members

        public T Wait()
        {
            return m_value; 
        }

        #endregion

        #region IFuture Members

        public bool HasCompleted
        {
            get{ return true; }
        }

        public void WaitEmpty()
        {
            // Do Nothing
        }

        #endregion
    }
}
