using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using BclExtras.Threading;

#if !NETFX_35
namespace System.Runtime.CompilerServices
{
    internal sealed class ExtensionAttribute : Attribute
    {

    }
}
#endif

namespace BclExtras
{
    /// <summary>
    /// Quick and dirty class built around DataEventArgs which only take a single
    /// piece of already defined data.  How many times have you written this class
    /// before?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Immutable]
    public sealed class DataEventArgs<T> : EventArgs
    {
        private readonly T m_data;

        public T Data
        {
            get { return m_data; }
        }

        public DataEventArgs(T data)
        {
            m_data = data;
        }
    }

    [SuppressMessage("Microsoft.Naming", "CA1711")]
    public static class DataEventArgs 
    {
        public static DataEventArgs<T> Create<T>(T data)
        {
            return new DataEventArgs<T>(data);
        }
    }

    /// <summary>
    /// Very simple event delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    [SuppressMessage("Microsoft.Design", "CA1003")]
    public delegate void DataEventCallback<T>(object sender, DataEventArgs<T> e); 

}
